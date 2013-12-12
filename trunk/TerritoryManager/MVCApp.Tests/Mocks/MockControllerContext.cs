using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;

// Used for mocking up authentication and authorization based on the roles

namespace MVCApp.Tests.Mocks
{
    class MockControllerContext : ControllerContext
    {
        HttpContextBase _context;

        public override HttpContextBase HttpContext
        {
            get
            {
                return _context;
            }
            set
            {
                _context = value;
            }
        }

        public MockControllerContext(string userName, HashSet<string> userRoles)
        {
            _context = new MockHttpContextBase(userName, userRoles);
        }
    }

    class MockHttpContextBase : HttpContextBase
    {
        IPrincipal _user;

        public override IPrincipal User
        {
            get 
            { 
                return _user;
            }
            set 
            { 
                _user = value;
            }
        }

        public MockHttpContextBase(string userName, HashSet<string> userRoles)
        {
            _user = new MockPrincipal(userName, userRoles);
        }
    }

    class MockPrincipal : IPrincipal
    {
        IIdentity _identity;
        string _userName;
        HashSet<string> _userRoles;

        public MockPrincipal(string userName, HashSet<string> userRoles)
        {
            _userName = userName;
            _userRoles = userRoles;
            _identity = new MockIdentity(userName);            
        }

        public bool IsInRole(string role)
        {
            if (_userRoles == null) 
                return false;
            return _userRoles.Contains(role);
        }

        public IIdentity Identity
        {
            get
            {
                return _identity;
            }
        }
    }

    class MockIdentity : IIdentity
    {
        private string _name;

        public MockIdentity(string name)
        {
            _name = name;
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string AuthenticationType { get { return null; } }
        public bool IsAuthenticated { get { return false; } }
    }
}
