namespace HowToSecureAPI.Demo.Constants
{
    public class ApiConstants
    {
        public const string AzureADInternalApplicationRoleClaimİdentifier = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        /// <summary> Scope claim identifier </summary>
        public const string B2CApplicationScopeClaimIdentifier = "http://schemas.microsoft.com/identity/claims/scope";

        // aplication id claims
        public const string AzureADInternalApplicationIdClaimİdentifier = "appid";
        public const string AzureB2CApplicationIdClaimİdentifier = "azp";

        public const string OktaApplicationSubClaimİdentifier = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public const string ApplicationIssuerClaim = "iss";
        /// <summary> Scope claim identifier </summary>
        public const string ApplicationScopeClaimIdentifier = "http://schemas.microsoft.com/identity/claims/scope";

        // B2C policy
        public const string UserReadPolicyName = "UserReadPolicy";
        public const string UsersReadScope = "Users.Read";
        public const string UsersManageScope = "Users.Manage";

        // Internal AD Policy
        public const string InternalUserReadPolicyName = "InternalUserReadPolicy";
        public const string InternalUserReadScope = "InternalUserReadScope";
        // B2C and Azure AD INTERNAL
        public const string InternalAndB2CUserReadPolicyName = "InternalAndB2CUserReadPolicy";


        //MultipleValidation B2C and Azure AD INTERNAL AND Okta
        public const string MultipleValidationUserReadPolicyName = "MultipleValidationUserReadPolicyName";
    }
}
