using Microsoft.EntityFrameworkCore;

namespace azure_cost_dashboard.Models
{
    public class User
    {
        public int Id { get; set; }
       public string UserName { get; set; }
       public string Password { get; set; }
    }
}
