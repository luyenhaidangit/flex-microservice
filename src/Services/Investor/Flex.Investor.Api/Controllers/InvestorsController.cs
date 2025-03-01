using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Investor.Api.Controllers
{
    public static class InvestorsController
    {
        public static void MapInvestorsApi(this WebApplication app)
        {
            var group = app.MapGroup("/api/investor");

            #region Query
            /// <summary>
            /// Phân trang danh sách nhà đầu tư.
            /// </summary>
            group.MapGet("/get-investors-paging", async ([AsParameters] GetInvestorsPagingRequest request, IInvestorService investorService) =>
            {
                var result = await investorService.GetPagingInvestorsAsync(request);

                return Results.Ok(Result.Success(result));
            });

            /// <summary>
            /// Thông tin chi tiết nhà đầu tư theo Id.
            /// </summary>
            group.MapGet("/get-investor-by-id", async ([AsParameters] EntityKey<long> entityKey, IInvestorService investorService) =>
            {
                var result = await investorService.GetInvestorByIdAsync(entityKey.Id);

                return Results.Ok(Result.Success(result));
            });

            /// <summary>
            /// Lấy danh sách tiểu khoản nhà đầu tư.
            /// </summary>
            group.MapGet("/get-subaccounts-by-investor-id", async ([AsParameters] EntityKey<long> entityKey, IInvestorService investorService) =>
            {
                var result = await investorService.GetSubAccountsByInvestorIdAsync(entityKey.Id);
                return Results.Ok(Result.Success(result));
            });
            #endregion

            #region Command
            /// <summary>
            /// Tạo mới nhà đầu tư.
            /// </summary>
            group.MapPost("/create-investor", async ([FromBody] CreateInvestorRequest request, IInvestorService investorService) =>
            {
                var result = await investorService.CreateInvestorAsync(request);

                if(!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }

                return Results.Ok(result);
            });

            /// <summary>
            /// Cập nhật nhà đầu tư.
            /// </summary>
            group.MapPost("/update-investor", async ([FromBody] UpdateInvestorRequest request, IInvestorService investorService) =>
            {
                var result = await investorService.UpdateInvestorAsync(request);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }

                return Results.Ok(result);
            });

            /// <summary>
            /// Xóa nhà đầu tư.
            /// </summary>
            group.MapPost("/delete-investor", async ([FromBody] EntityKey<long> entityKey, IInvestorService investorService) =>
            {
                // Validate if investor exists before deletion
                var result = await investorService.DeleteInvestorAsync(entityKey.Id);

                if (!result.IsSuccess)
                {
                    return Results.BadRequest(result);
                }

                return Results.Ok(result);
            });

            /// <summary>
            /// Thêm mới tiểu khoản.
            /// </summary>
            group.MapPost("/create-subaccount", async ([FromBody] CreateSubAccountRequest request, IInvestorService investorService) =>
            {
                var result = await investorService.CreateSubAccountAsync(request);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            /// <summary>
            /// Cập nhật tiểu khoản.
            /// </summary>
            group.MapPost("/update-subaccount", async ([FromBody] UpdateSubAccountRequest request, IInvestorService investorService) =>
            {
                var result = await investorService.UpdateSubAccountAsync(request);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });

            /// <summary>
            /// Xóa tiểu khoản.
            /// </summary>
            group.MapPost("/delete-subaccount", async ([FromBody] EntityKey<long> entityKey, IInvestorService investorService) =>
            {
                var result = await investorService.DeleteSubAccountAsync(entityKey.Id);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            });
            #endregion
        }
    }
}
