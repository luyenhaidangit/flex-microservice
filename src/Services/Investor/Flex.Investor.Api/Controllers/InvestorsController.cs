using Flex.Investor.Api.Services.Interfaces;
using Flex.Shared.DTOs.Investor;
using Flex.Shared.SeedWork;

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
            #endregion

            #region Command
            /// <summary>
            /// Create a new investor.
            /// </summary>
            //group.MapPost("/create-investor", async (CreateInvestorRequest request, IInvestorService investorService) =>
            //{
            //    // Validate if email already exists
            //    var result = await investorService.CreateInvestorAsync(request);
            //    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            //});

            /// <summary>
            /// Update investor details.
            /// </summary>
            //group.MapPut("/update-investor", async (UpdateInvestorRequest request, IInvestorService investorService) =>
            //{
            //    // Validate if investor exists before updating
            //    var result = await investorService.UpdateInvestorAsync(request);
            //    return result.IsSuccess ? Results.Ok(result) : Results.Conflict(result);
            //});

            /// <summary>
            /// Delete an investor.
            /// </summary>
            //group.MapDelete("/delete-investor", async (long id, IInvestorService investorService) =>
            //{
            //    // Validate if investor exists before deletion
            //    var result = await investorService.DeleteInvestorAsync(id);
            //    return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            //});
            #endregion
        }
    }
}
