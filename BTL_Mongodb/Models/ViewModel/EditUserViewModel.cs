using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BTL_Mongodb.Models.ViewModel
{
    public class EditUserViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  = ObjectId.GenerateNewId().ToString();
        public string Username { get; set; }
        public string Email { get; set; }

        public List<string> SelectedRoleIds { get; set; } = new List<string>();
        [ValidateNever]
        public List<Role> AllRoles { get; set; }
    }

}
