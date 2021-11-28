using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Repositories;

namespace PaymentContext.Tests.Mocks
{
    // depender sempre da abstração não da implementação, fica mais facil testar
    public class FakeStudentRepository : IStudentRepository
    {
        public bool DocumentExists(string document) => document == "99999999999";

        public bool EmailExists(string email) => email == "teste@teste.com";

        public void CreateSubscription(Student student)
        {
            
        }
    }
}