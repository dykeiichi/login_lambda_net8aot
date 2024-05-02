using System.Collections.Generic;
using System.IO;
using Amazon.DynamoDBv2.Model;
using LoginLambda.Models;

namespace Shared.DataAccess
{
    public class UserMapper
    {
        public const string PK = "Username";
        public const string PASSWORD = "Password";
        public const string SALT = "Salt";
        
        public static UserContext UserFromDynamoDB(Dictionary<string, AttributeValue> items) {
            UserContext user = new (items[PK].S, items[PASSWORD].B.ToArray(), items[SALT].B.ToArray());
            return user;
        }
        
        public static Dictionary<string, AttributeValue> UserToDynamoDb(UserContext user) {
            Dictionary<string, AttributeValue> item = new(3)
            {
                { PK, new AttributeValue(user.Username) },
                {
                    PASSWORD,
                    new AttributeValue()
                    {
                        B = new MemoryStream(user.Password)
                    }
                },
                {
                    SALT,
                    new AttributeValue()
                    {
                        B = new MemoryStream(user.Salt)
                    }
                }
            };

            return item;
        }
    }
}