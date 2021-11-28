using System;
using Flunt.Notifications;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Service;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Handler;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable<Notification>, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }
        
        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            //fail fast validations
            command.Validate();
            if (!command.IsValid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            }
            
            //verificar se Documento já está cadastrado
            if(_repository.DocumentExists(command.Documment))
                AddNotification("Document", "Este CPF já está em uso");
            
            // verificar se e-mail já está cadastrado
            if(_repository.EmailExists(command.Email))
                AddNotification("Email", "Este email já está em uso");
            
            //Gerar os VOs
            
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Documment, EDocumentType.CPF);
             var email = new Email(command.Email);
             var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
             
             
            
            
            //Gerar as entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate,
                command.Total, command.TotalPaid, command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType), address, new Email(command.PayerEmail));
            
            
            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);
            
            // Agrupar as validações
            AddNotifications(name, document, email, address, student, subscription, payment);
            
            // Checar as notificações
            if (!IsValid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");

            
            // Salvar as informações
            _repository.CreateSubscription(student);
            
            // Enviar E-mail de boas vindas
            
            _emailService.Send(student.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");
            
            //Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {
                        //fail fast validations
            command.Validate();
            if (!command.IsValid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            }
            
            //verificar se Documento já está cadastrado
            if(_repository.DocumentExists(command.Documment))
                AddNotification("Document", "Este CPF já está em uso");
            
            // verificar se e-mail já está cadastrado
            if(_repository.EmailExists(command.Email))
                AddNotification("Email", "Este email já está em uso");
            
            //Gerar os VOs
            
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Documment, EDocumentType.CPF);
             var email = new Email(command.Email);
             var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);
             
             
            
            
            //Gerar as entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(command.TransactionCode, command.PaidDate, command.ExpireDate,
                command.Total, command.TotalPaid, command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType), address, new Email(command.PayerEmail));
            
            
            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);
            
            // Agrupar as validações
            AddNotifications(name, document, email, address, student, subscription, payment);
            
            // Checar as notificações
            if (!IsValid)
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            
            // Salvar as informações
            _repository.CreateSubscription(student);
            
            // Enviar E-mail de boas vindas
            
            _emailService.Send(student.ToString(), student.Email.Address, "Bem vindo ao balta.io", "Sua assinatura foi criada");
            
            //Retornar informações

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}