using StashBot.Models.ReturnModels.ReturnStatusEnums;

namespace StashBot.Models.ReturnModels.ViewCommandHandlerReturns
{
    public class ViewInvokeReturn : ReturnModelBase
    {
        public ViewInvokeReturnStatus Status { get; set; }
    }
}