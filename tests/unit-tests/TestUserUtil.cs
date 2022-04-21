using Unico.Admin.Api.Models;

namespace unit_tests
{
    static class TestUserUtil
    {

        public static UserDto CreateTestUserSuperMario()
        {
            UserDto user = new UserDto
            {
                givenName = "Mario",
                sureName = "Super",
                name = "Super Mario",
                emailAddress = "super.mario@unico.ch",
                samAccountName = "superMario",
                userPrincipalName = "superMario",
                path = "OU=Münsingen,OU=UAdminPortal,OU=Hosting,DC=unico-adminportal-dev-ad,DC=switzerlandnorth,DC=cloudapp,DC=azure,DC=com",
            };
            return user;
        }
    }
}
