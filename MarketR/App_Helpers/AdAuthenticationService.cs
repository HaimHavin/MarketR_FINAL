using System.Web.Security;

namespace MarketR
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.IO;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    public class AdAuthenticationService
    {
        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                this.ErrorMessage = errorMessage;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(this.ErrorMessage);
        }

        /// <summary>
        /// Check if username and password matches existing account in AD. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AuthenticationResult SignIn(String username, String password)
        {

            ContextType authenticationType = ContextType.Domain;
#if DEBUG
            // authenticates against your local machine - for development time
           // ContextType authenticationType = ContextType.Machine;
            //PrincipalContext principalContext = new PrincipalContext(authenticationType, "13.65.252.202", "corp\\Marketr.admin", "Marketr.admin1988");
            // PrincipalContext principalContext = new PrincipalContext(authenticationType, "13.84.163.87", "corp\\iryna", "Qwertyui1234");
            PrincipalContext principalContext = new PrincipalContext(authenticationType, "192.168.1.201", username, password);

#else
// authenticates against your Domain AD
            
            PrincipalContext principalContext = new PrincipalContext(authenticationType);
             
#endif
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;
            try
            {
                //   isAuthenticated = Membership.Providers["ADMembershipProvider"].ValidateUser(
                //    username, password);

                isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                //var ss = Membership.GetUser(username);
                if (isAuthenticated)
                {
                    userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
                }
            }
            catch (Exception ex)
            {
                isAuthenticated = false;
                userPrincipal = null;
            }

            if (!isAuthenticated || userPrincipal == null)
            {
                return new AuthenticationResult("Username or Password is not correct");
            }

            if (userPrincipal.IsAccountLockedOut())
            {
                // here can be a security related discussion weather it is worth 
                // revealing this information
                return new AuthenticationResult("Your account is locked.");
            }

            if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
            {
                // here can be a security related discussion weather it is worth 
                // revealing this information
                return new AuthenticationResult("Your account is disabled");
            }
            return new AuthenticationResult();
        }
    }
}