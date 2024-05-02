using System;
using System.Linq;
using Libsodium = LoginLambda.Cryptography.Libsodium;

namespace LoginLambda.Models
{
    public class UserContext {

        public static readonly UserContext Empty;

        static UserContext() {
            Empty = new UserContext();
        }
        

        public string Username { get{ return username; } set{ username = value; } }
        private string username;
        private readonly byte[] password;
        public byte[] Password { get { return password; }}
        public byte[] Salt { get{ return salt; } set { salt = value; } }
        private byte[] salt;

        public UserContext(string Username, byte[] Password, byte[] Salt) {
            this.username = Username;
            this.password = Password;
            this.salt = Salt;
        }

        private UserContext() {
            this.username = "";
            this.password = [];
            this.salt = [];
        }

        public static bool SignUp(User user, out UserContext UserContext) {
            try {
                byte[] Salt = Libsodium.CreateSalt();
                byte[] password = Libsodium.HashPassword(user.Password, Salt, Salt.Length);
                UserContext = new(user.Username, password, Salt);
                return true;
            } catch (Exception) {
                UserContext = Empty;
                return false;
            }
        }

        public bool SignIn(string PlainTextPassword) {
            try {
                byte[] HashedPassword = Libsodium.HashPassword(PlainTextPassword, Salt, Salt.Length);
                return HashedPassword.SequenceEqual(password);
            } catch (Exception) {
                return false;
            }
        }

    }
}