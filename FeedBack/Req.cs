namespace FeedBack
{
    public class ReqRegister
    {
        required
        public string username { get; set; } = null!;

        public string fullname { get; set; } = null!;

        public string email { get; set; } = null!;

        public string password { get; set; } = null!;
    }

    public class ReqLogin
    {
        required
        public string email { get; set; } = null!;

        public string password { get; set; } = null!;
    }

    public class ReqAddUser
    {
        required
        public string username { get; set; } = null!;

        public string fullname { get; set; } = null!;

        public string email { get; set; } = null!;

        public string password { get; set; } = null!;

        public UserRole role { get; set; }
    }

    public class ReqUpdateUser
    {
        required
        public string username { get; set; } = null!;

        public string fullname { get; set; } = null!;

        public string email { get; set; } = null!;

        public string password { get; set; } = null!;

        public UserRole role { get; set; }
    }

    public class ReqAddProduct
    {
        required
        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public int SuperCategoryId { get; set; } 

    }

    public class ReqUpdateProduct
    {
        required

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }
    }


    public enum UserRole
    {
        Admin,
        Marketing,
    }
}
