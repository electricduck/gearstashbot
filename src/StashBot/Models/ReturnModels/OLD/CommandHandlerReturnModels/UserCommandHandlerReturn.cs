
namespace StashBot.Models.ReturnModels.CommandHandlerReturnModels
{
    public class UserCommandHandlerReturn : CommandHandlerReturnBase
    {
        public string Permission { get; set; }
        public UserCommandReturnStatus Status { get; set; }

        public enum UserCommandReturnStatus
        {
            CannotChangeForSelf = 5,
            CannotDeleteSelf = 7,
            CreatedUser = 3,
            DeletedUser = 1,
            EditedUserPermissions = 9,
            GotUser = 10,
            InvalidArgs = 0,
            Unauthorized = 6,
            UserAlreadyExists = 8,
            UserNotFound = 4
        }
    }
}