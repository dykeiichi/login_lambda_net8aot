using System.Diagnostics.CodeAnalysis;

namespace LoginLambda.Models
{
    public class User
    {

        public static readonly User Empty = new ();
        public string Username { get; set; }
        public string Password { get; set; }

        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(User))]
        public User() {
            Username = string.Empty;
            Password = string.Empty;  
        }

        public User(string username, string password) {
            Username = username;
            Password = password;
        }

        public override string ToString()
        {
            return "User{" +
                   "Username='" + this.Username + '\'' +
                   ", Password='" + this.Password + '\'' +
                   '}';
        }
    }
}