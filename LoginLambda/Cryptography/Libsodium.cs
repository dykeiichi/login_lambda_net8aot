// using DllImport = System.Runtime.InteropServices.DllImportAttribute; // use this on dotnet version less than 8
using LibraryImport = System.Runtime.InteropServices.LibraryImportAttribute;
// using CallingConvention = System.Runtime.InteropServices.CallingConvention; // use this on dotnet version less than 8
using StringBuilder = System.Text.StringBuilder;
using SHA512 = System.Security.Cryptography.SHA512;
using Exception = System.Exception;
using System.Linq;

namespace LoginLambda.Cryptography
{
    public partial class Libsodium {
        private const string Name = "libsodium";
        private static int id_ALG_ARGON2ID13 = 2;
        private static long OPSLIMIT_SENSITIVE = 2;
        private static int MEMLIMIT_SENSITIVE = 19922944;

        static Libsodium() {
            sodium_init();
        }

        // use this on dotnet version less than 8
        // [DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
        [LibraryImport(Name, EntryPoint = "sodium_init")]
        internal static partial void sodium_init();

        // use this on dotnet version less than 8
        //[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
        [LibraryImport(Name, EntryPoint = "randombytes_buf")]
        internal static partial void randombytes_buf(byte[] buffer, int size);

        // use this on dotnet version less than 8
        //[DllImport(Name, CallingConvention = CallingConvention.Cdecl)]
        [LibraryImport(Name, EntryPoint = "crypto_pwhash")]
        internal static partial int crypto_pwhash(byte[] buffer, long bufferLen, byte[] password, long passwordLen, byte[] salt, long opsLimit, int memLimit, int alg);

        public static byte[] CreateSalt(int bytes_size = 16) {
            byte[] buffer = new byte[bytes_size];
            randombytes_buf(buffer, buffer.Length);
            return buffer;
        }

        public static byte[] HashPassword(string password, byte[] salt, int bytes_size = 16) {
            byte[] hash = new byte[bytes_size];

            int result = crypto_pwhash(
                hash,
                hash.Length,
                System.Text.Encoding.UTF8.GetBytes(password),
                password.Length,
                salt,
                OPSLIMIT_SENSITIVE,
                MEMLIMIT_SENSITIVE,
                id_ALG_ARGON2ID13
                );

            if (result != 0)
                throw new Exception("An unexpected error has occurred.");

            byte[] hashedInputBytes = SHA512.HashData(hash);
            StringBuilder hashedInputStringBuilder = new (128);
                foreach (byte b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("X2"));
                return System.Text.Encoding.UTF8.GetBytes(hashedInputStringBuilder.ToString());
        }

        public static bool VerifyHash(string password, byte[] salt, byte[] hash) {
            byte[] newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}