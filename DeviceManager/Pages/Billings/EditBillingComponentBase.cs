using DeviceManager.Pages.Patient;
using DeviceManager.Repository;
using Entities;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace DeviceManager.Pages.Billings
{
	public class EditBillingComponentBase : ComponentBase
	{

        public BillingEntityDto	 billingEntityDto { get; set; }
		private BillingEntity billingEntity { get; set; }
		protected MudForm form;
		protected bool pageLoaded { get; set; } = false;

		[Parameter]
        public string BillingId { get; set; }
		[Inject] ISnackbar Snackbar { get; set; }

		[Inject]
		protected IBillingDetailsRepository billingDetailsRepository { get; set; }
		[Inject]
		protected IBillingRepository billingRepository { get; set; }
		[Inject]
		NavigationManager navigationManager { get; set; }

		protected override async Task OnInitializedAsync()
		{
			
			billingEntity = await billingRepository.GetBillingById(Guid.Parse(BillingId));
			if (billingEntity != null)
			{
				billingEntityDto = new BillingEntityDto()
				{
					BillingDetails = billingEntity.BillingDetails == null ? new BillingDetailsEntity() : billingEntity.BillingDetails,
					BillingId = billingEntity.BillingId,
				};
			}
			pageLoaded = true;
			await base.OnInitializedAsync();
		}

		protected async Task Submit()
		{
			await form.Validate();
			if (form.IsValid)
			{
				Snackbar.Add("Updated!");
				if(billingEntityDto.BillingDetails == null  || billingEntityDto.BillingDetails.BillingDetailsId == null || billingEntityDto.BillingDetails.BillingDetailsId.Equals(Guid.Empty))
				{
					await billingDetailsRepository.AddBillingDetails(billingEntityDto.BillingDetails, billingEntity);

					navigationManager.NavigateTo("/billing");
				}
				else if (await billingDetailsRepository.UpdateBilling(billingEntityDto.BillingDetails, billingEntity))
				{
					navigationManager.NavigateTo("/billing");
				}
			}
		}

		private async Task<bool> updateBilling()
		{
			return await billingRepository.UpdateBilling(billingEntity);
		}
		protected void Cancel()
		{
			navigationManager.NavigateTo("/billing");
		}
	}
}
