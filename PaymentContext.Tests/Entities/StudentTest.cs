using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;

namespace PaymentContext.Tests.Entities
{
    [TestClass]
    public class StudentTest
    {
        [TestMethod]
        public void TestMethod(){
            var subscription = new Subscription(null);
            var student = new Student("Lucas", "Robaert", "2222", "lucas@robaert.com.br");
            student.AddSubscription(subscription);
        }
    }
}