using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LoginLambda.Models
{
    public class UserWrapper
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(UserWrapper))]
        public UserWrapper(){
            Users = [];
        }

        public UserWrapper(List<UserContext> users)
        {
            this.Users = users;
        }
        
        public List<UserContext> Users { get; set; }
    }
}