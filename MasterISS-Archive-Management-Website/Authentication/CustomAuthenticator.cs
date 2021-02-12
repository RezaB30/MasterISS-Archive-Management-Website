using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RezaB.Web.Authentication;
using RadiusR.DB;
using System.Security.Cryptography;
using System.Linq.Expressions;

namespace MasterISS_Archive_Management_Website.Authentication
{
    public class CustomAuthenticator : Authenticator<RadiusREntities, AppUser, Role, Permission, SHA256>
        {
            public CustomAuthenticator() : base(u => u.ID, u => u.Name, u => u.Email, u => u.Password, u => u.IsEnabled, r => r.Name, p => p.Name) { }

        }    
}