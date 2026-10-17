using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace MiniSupermarket.API.Controllers
{
    public class CheckoutItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class CheckoutRequestDto
    {
        public int? CustomerId { get; set; }
        public string CashierUsername { get; set; } = string.Empty;
        public List<CheckoutItemDto> Items { get; set; } = new();
    }

    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CheckoutController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string GetConnectionString()
        {
            return _configuration.GetConnectionString("DefaultConnection")!;
        }

        // =========================================================================
        // BUỔI 5 - TIẾT 5: THANH TOÁN POS BỌC TRONG SQLTRANSACTION (CHUẨN ACID)
        // =========================================================================
        // POST: api/checkout
        [HttpPost]
        public IActionResult ProcessCheckout([FromBody] CheckoutRequestDto request)
        {
            // 1. Kiểm tra tính hợp lệ của giỏ hàng
            if (request.Items == null || request.Items.Count == 0)
            {
                return BadRequest(new { success = false, message = "Giỏ hàng rỗng, không thể tiến hành thanh toán!" });
            }

            if (request.Items.Any(i => i.Quantity <= 0))
            {
                return BadRequest(new { success = false, message = "Số lượng mua của mỗi sản phẩm phải lớn hơn 0!" });
            }

            if (string.IsNullOrWhiteSpace(request.CashierUsername))
            {
                return BadRequest(new { success = false, message = "Thiếu thông tin tài khoản thu ngân lập hóa đơn!" });
            }

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                // 2. KHỞI TẠO TRANSACTION (Bắt đầu phiên giao dịch khép kín bảo vệ ACID)
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Lấy thông tin thực tế từ database (Tên và Đơn giá chính xác)
                    var processedItems = new List<(int ProductId, string ProductName, decimal UnitPrice, int Quantity)>();
                    decimal finalAmount = 0;
                    int totalItems = 0;

                    foreach (var item in request.Items)
                    {
                        // 3.1. Kiểm tra số lượng tồn kho thực tế trước khi trừ
                        string checkStockSql = "SELECT StockQuantity, ProductName, Price FROM Products WHERE ProductId = @ProductId";
                        int currentStock = 0;
                        string productName = string.Empty;
                        decimal unitPrice = item.UnitPrice;

                        using (SqlCommand cmdCheck = new SqlCommand(checkStockSql, connection, transaction))
                        {
                            cmdCheck.Parameters.AddWithValue("@ProductId", item.ProductId);
                            using (SqlDataReader reader = cmdCheck.ExecuteReader())
                            {
                                if (!reader.Read())
                                {
                                    throw new InvalidOperationException($"Không tìm thấy sản phẩm có ID: {item.ProductId}");
                                }
                                currentStock = Convert.ToInt32(reader["StockQuantity"]);
                                productName = reader["ProductName"]?.ToString() ?? string.Empty;
                                if (unitPrice <= 0)
                                {
                                    unitPrice = Convert.ToDecimal(reader["Price"]);
                                }
                            }
                        }

                        // 3.2. ĐIỂM KIỂM SOÁT LOGIC: Nếu số lượng mua vượt quá tồn kho -> Kích hoạt Exception
                        if (currentStock < item.Quantity)
                        {
                            throw new InvalidOperationException(
                                $"Sản phẩm '{productName}' (ID: {item.ProductId}) không đủ tồn kho! " +
                                $"(Tồn kho thực tế: {currentStock}, Số lượng yêu cầu mua: {item.Quantity})");
                        }

                        processedItems.Add((item.ProductId, productName, unitPrice, item.Quantity));
                        finalAmount += unitPrice * item.Quantity;
                        totalItems += item.Quantity;
                    }

                    // Quy tắc tích điểm siêu thị: Cứ mỗi 10.000đ mua hàng được tặng 1 điểm thưởng
                    int rewardPoints = (int)(finalAmount / 10000m);

                    // =====================================================================
                    // BƯỚC 1: TẠO HÓA ĐƠN TỔNG THỂ TRONG BẢNG Orders
                    // =====================================================================
                    string insertOrderSql = @"
                        INSERT INTO Orders (OrderDate, CashierUsername, CustomerId, TotalAmount, PointsEarned)
                        OUTPUT INSERTED.OrderId
                        VALUES (GETDATE(), @CashierUsername, @CustomerId, @TotalAmount, @PointsEarned)";

                    int orderId = 0;
                    using (SqlCommand cmdOrder = new SqlCommand(insertOrderSql, connection, transaction))
                    {
                        cmdOrder.Parameters.AddWithValue("@CashierUsername", request.CashierUsername.Trim());
                        cmdOrder.Parameters.AddWithValue("@CustomerId", (object?)request.CustomerId ?? DBNull.Value);
                        cmdOrder.Parameters.AddWithValue("@TotalAmount", finalAmount);
                        cmdOrder.Parameters.AddWithValue("@PointsEarned", rewardPoints);

                        orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());
                    }

                    // =====================================================================
                    // BƯỚC 2 & 3: XỬ LÝ TỪNG MÓN HÀNG (LƯU CHI TIẾT VÀ TRỪ TỒN KHO)
                    // =====================================================================
                    foreach (var pItem in processedItems)
                    {
                        // 3.3. Thêm dòng chi tiết vào bảng OrderDetails
                        string insertDetailSql = @"
                            INSERT INTO OrderDetails (OrderId, ProductId, ProductName, UnitPrice, Quantity)
                            VALUES (@OrderId, @ProductId, @ProductName, @UnitPrice, @Quantity)";

                        using (SqlCommand cmdDetail = new SqlCommand(insertDetailSql, connection, transaction))
                        {
                            cmdDetail.Parameters.AddWithValue("@OrderId", orderId);
                            cmdDetail.Parameters.AddWithValue("@ProductId", pItem.ProductId);
                            cmdDetail.Parameters.AddWithValue("@ProductName", pItem.ProductName);
                            cmdDetail.Parameters.AddWithValue("@UnitPrice", pItem.UnitPrice);
                            cmdDetail.Parameters.AddWithValue("@Quantity", pItem.Quantity);
                            cmdDetail.ExecuteNonQuery();
                        }

                        // 3.4. Khấu trừ số lượng tồn kho trong bảng Products
                        string updateStockSql = @"
                            UPDATE Products 
                            SET StockQuantity = StockQuantity - @Qty 
                            WHERE ProductId = @ProductId";

                        using (SqlCommand cmdStock = new SqlCommand(updateStockSql, connection, transaction))
                        {
                            cmdStock.Parameters.AddWithValue("@Qty", pItem.Quantity);
                            cmdStock.Parameters.AddWithValue("@ProductId", pItem.ProductId);
                            cmdStock.ExecuteNonQuery();
                        }
                    }

                    // =====================================================================
                    // BƯỚC 4: CỘNG ĐIỂM TÍCH LŨY CHO KHÁCH HÀNG (NẾU CÓ)
                    // =====================================================================
                    if (request.CustomerId.HasValue && request.CustomerId.Value > 0 && rewardPoints > 0)
                    {
                        string updatePointsSql = @"
                            UPDATE Customers 
                            SET RewardPoints = RewardPoints + @Points 
                            WHERE CustomerId = @CustomerId";

                        using (SqlCommand cmdCustomer = new SqlCommand(updatePointsSql, connection, transaction))
                        {
                            cmdCustomer.Parameters.AddWithValue("@Points", rewardPoints);
                            cmdCustomer.Parameters.AddWithValue("@CustomerId", request.CustomerId.Value);
                            cmdCustomer.ExecuteNonQuery();
                        }
                    }

                    // =====================================================================
                    // BƯỚC 5: XÁC NHẬN LƯU VĨNH VIỄN TOÀN BỘ THAO TÁC (COMMIT)
                    // =====================================================================
                    transaction.Commit();

                    return Ok(new
                    {
                        success = true,
                        message = "Thanh toán hóa đơn POS thành công!",
                        orderId = orderId,
                        cashierUsername = request.CashierUsername,
                        customerId = request.CustomerId,
                        totalItems = totalItems,
                        finalAmount = finalAmount,
                        rewardPointsAdded = rewardPoints
                    });
                }
                catch (Exception ex)
                {
                    // =====================================================================
                    // BƯỚC 6: HOÀN TÁC TOÀN BỘ DỮ LIỆU SẠCH SẼ KHI CÓ BẤT KỲ LỖI NÀO (ROLLBACK)
                    // =====================================================================
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        return StatusCode(500, new
                        {
                            success = false,
                            message = "Lỗi nghiêm trọng khi thực thi Rollback: " + rollbackEx.Message
                        });
                    }

                    return BadRequest(new
                    {
                        success = false,
                        message = "Giao dịch thanh toán bị hủy và đã được Rollback an toàn!",
                        errorDetail = ex.Message
                    });
                }
            }
        }

        // =========================================================================
        // BÀI TẬP MỞ RỘNG (TIẾT 5): HỦY ĐƠN HÀNG VÀ HOÀN TRẢ TỒN KHO (TRANSACTION)
        // =========================================================================
        // DELETE: api/checkout/cancel/{orderId}
        [HttpDelete("cancel/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // 1. Kiểm tra hóa đơn và lấy thông tin khách hàng, điểm thưởng
                    string getOrderSql = "SELECT CustomerId, PointsEarned FROM Orders WHERE OrderId = @OrderId";
                    int? customerId = null;
                    int pointsToDeduct = 0;

                    using (SqlCommand cmdGet = new SqlCommand(getOrderSql, connection, transaction))
                    {
                        cmdGet.Parameters.AddWithValue("@OrderId", orderId);
                        using (SqlDataReader reader = cmdGet.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                throw new InvalidOperationException($"Không tìm thấy hóa đơn ID = {orderId} để hủy!");
                            }
                            if (reader["CustomerId"] != DBNull.Value)
                            {
                                customerId = Convert.ToInt32(reader["CustomerId"]);
                            }
                            pointsToDeduct = Convert.ToInt32(reader["PointsEarned"]);
                        }
                    }

                    // 2. Lấy danh sách sản phẩm đã mua để hoàn trả kho
                    var itemsToRestore = new List<(int ProductId, int Quantity)>();
                    string getDetailsSql = "SELECT ProductId, Quantity FROM OrderDetails WHERE OrderId = @OrderId";

                    using (SqlCommand cmdDetails = new SqlCommand(getDetailsSql, connection, transaction))
                    {
                        cmdDetails.Parameters.AddWithValue("@OrderId", orderId);
                        using (SqlDataReader reader = cmdDetails.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                itemsToRestore.Add((
                                    Convert.ToInt32(reader["ProductId"]),
                                    Convert.ToInt32(reader["Quantity"])
                                ));
                            }
                        }
                    }

                    // 3. Hoàn trả tồn kho trong bảng Products
                    foreach (var item in itemsToRestore)
                    {
                        string restoreStockSql = @"
                            UPDATE Products 
                            SET StockQuantity = StockQuantity + @Qty 
                            WHERE ProductId = @ProductId";

                        using (SqlCommand cmdRestore = new SqlCommand(restoreStockSql, connection, transaction))
                        {
                            cmdRestore.Parameters.AddWithValue("@Qty", item.Quantity);
                            cmdRestore.Parameters.AddWithValue("@ProductId", item.ProductId);
                            cmdRestore.ExecuteNonQuery();
                        }
                    }

                    // 4. Khấu trừ điểm thưởng đã tích của khách hàng
                    if (customerId.HasValue && pointsToDeduct > 0)
                    {
                        string deductPointsSql = @"
                            UPDATE Customers 
                            SET RewardPoints = CASE 
                                WHEN RewardPoints >= @Pts THEN RewardPoints - @Pts 
                                ELSE 0 
                            END 
                            WHERE CustomerId = @CustomerId";

                        using (SqlCommand cmdDeduct = new SqlCommand(deductPointsSql, connection, transaction))
                        {
                            cmdDeduct.Parameters.AddWithValue("@Pts", pointsToDeduct);
                            cmdDeduct.Parameters.AddWithValue("@CustomerId", customerId.Value);
                            cmdDeduct.ExecuteNonQuery();
                        }
                    }

                    // 5. Xóa chi tiết hóa đơn trước (tránh vi phạm ràng buộc khóa ngoại)
                    string deleteDetailsSql = "DELETE FROM OrderDetails WHERE OrderId = @OrderId";
                    using (SqlCommand cmdDelDetails = new SqlCommand(deleteDetailsSql, connection, transaction))
                    {
                        cmdDelDetails.Parameters.AddWithValue("@OrderId", orderId);
                        cmdDelDetails.ExecuteNonQuery();
                    }

                    // 6. Xóa hóa đơn tổng trong bảng Orders
                    string deleteOrderSql = "DELETE FROM Orders WHERE OrderId = @OrderId";
                    using (SqlCommand cmdDelOrder = new SqlCommand(deleteOrderSql, connection, transaction))
                    {
                        cmdDelOrder.Parameters.AddWithValue("@OrderId", orderId);
                        cmdDelOrder.ExecuteNonQuery();
                    }

                    // 7. Xác nhận lưu vĩnh viễn (Commit)
                    transaction.Commit();

                    return Ok(new
                    {
                        success = true,
                        message = $"Đã hủy thành công hóa đơn #{orderId} và hoàn trả toàn bộ tồn kho vào CSDL!"
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new
                    {
                        success = false,
                        message = "Hủy hóa đơn thất bại, giao dịch đã được Rollback!",
                        errorDetail = ex.Message
                    });
                }
            }
        }
    }
}
