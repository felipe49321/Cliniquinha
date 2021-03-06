﻿using Clinica_V3._0.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Clinica_V3._0.Startup))]
namespace Clinica_V3._0
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            createRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login   
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            if (!roleManager.RoleExists("Administrador"))
            {

                // first we create Admin role   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Administrador";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                 

                var user = new ApplicationUser();

                user.UserName = "admin@admin.com";
                user.Email = "admin@admin.com";
                user.Administrador = new Administrador
                {
                    Nome = "administrador",
                    Endereco = "Endereço Administrador",
                    Rg = 1212121212,
                    Telefone = "(51)32123234"                  
                };
                string userPWD = "senha1234";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin  
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Administrador");

                }
            }

            // creating Creating Medico role    
            if (!roleManager.RoleExists("Medico"))
            {
                var role = new IdentityRole();
                role.Name = "Medico";
                roleManager.Create(role);

            }

            // creating Creating Secretaria role    
            if (!roleManager.RoleExists("Secretaria"))
            {
                var role = new IdentityRole();
                role.Name = "Secretaria";
                roleManager.Create(role);

            }
        }
    }
}
