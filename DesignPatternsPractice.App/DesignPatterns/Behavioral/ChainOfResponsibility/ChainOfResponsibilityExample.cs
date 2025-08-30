namespace DesignPatterns.Behavioral.ChainOfResponsibility
{
    // Request class
    public class ExpenseRequest
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }

        public ExpenseRequest(string name, decimal amount, string purpose)
        {
            Name = name;
            Amount = amount;
            Purpose = purpose;
        }
    }

    // Abstract Handler
    public abstract class ExpenseHandler
    {
        protected ExpenseHandler? _nextHandler;

        public void SetNext(ExpenseHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public abstract void HandleRequest(ExpenseRequest request);
    }

    // Concrete Handlers
    public class TeamLeader : ExpenseHandler
    {
        public override void HandleRequest(ExpenseRequest request)
        {
            if (request.Amount <= 1000)
            {
                Console.WriteLine($"Team Leader approved expense: {request.Name} - ${request.Amount} for {request.Purpose}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Team Leader cannot approve ${request.Amount}. Forwarding to next level.");
                _nextHandler.HandleRequest(request);
            }
            else
            {
                Console.WriteLine($"No one can approve expense: {request.Name} - ${request.Amount}");
            }
        }
    }

    public class Manager : ExpenseHandler
    {
        public override void HandleRequest(ExpenseRequest request)
        {
            if (request.Amount <= 5000)
            {
                Console.WriteLine($"Manager approved expense: {request.Name} - ${request.Amount} for {request.Purpose}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Manager cannot approve ${request.Amount}. Forwarding to next level.");
                _nextHandler.HandleRequest(request);
            }
            else
            {
                Console.WriteLine($"No one can approve expense: {request.Name} - ${request.Amount}");
            }
        }
    }

    public class Director : ExpenseHandler
    {
        public override void HandleRequest(ExpenseRequest request)
        {
            if (request.Amount <= 20000)
            {
                Console.WriteLine($"Director approved expense: {request.Name} - ${request.Amount} for {request.Purpose}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Director cannot approve ${request.Amount}. Forwarding to next level.");
                _nextHandler.HandleRequest(request);
            }
            else
            {
                Console.WriteLine($"No one can approve expense: {request.Name} - ${request.Amount}");
            }
        }
    }

    public class CEO : ExpenseHandler
    {
        public override void HandleRequest(ExpenseRequest request)
        {
            Console.WriteLine($"CEO approved expense: {request.Name} - ${request.Amount} for {request.Purpose}");
        }
    }

    // Real-world example: Support Ticket System
    public enum TicketLevel
    {
        Level1,
        Level2,
        Level3,
        Management
    }

    public class SupportTicket
    {
        public string Issue { get; set; }
        public TicketLevel Level { get; set; }
        public string Customer { get; set; }

        public SupportTicket(string customer, string issue, TicketLevel level)
        {
            Customer = customer;
            Issue = issue;
            Level = level;
        }
    }

    public abstract class SupportHandler
    {
        protected SupportHandler? _nextHandler;

        public void SetNext(SupportHandler nextHandler)
        {
            _nextHandler = nextHandler;
        }

        public abstract void HandleTicket(SupportTicket ticket);
    }

    public class Level1Support : SupportHandler
    {
        public override void HandleTicket(SupportTicket ticket)
        {
            if (ticket.Level == TicketLevel.Level1)
            {
                Console.WriteLine($"Level 1 Support resolved: {ticket.Issue} for {ticket.Customer}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Level 1 Support escalating ticket to next level");
                _nextHandler.HandleTicket(ticket);
            }
        }
    }

    public class Level2Support : SupportHandler
    {
        public override void HandleTicket(SupportTicket ticket)
        {
            if (ticket.Level == TicketLevel.Level2)
            {
                Console.WriteLine($"Level 2 Support resolved: {ticket.Issue} for {ticket.Customer}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Level 2 Support escalating ticket to next level");
                _nextHandler.HandleTicket(ticket);
            }
        }
    }

    public class Level3Support : SupportHandler
    {
        public override void HandleTicket(SupportTicket ticket)
        {
            if (ticket.Level == TicketLevel.Level3)
            {
                Console.WriteLine($"Level 3 Support resolved: {ticket.Issue} for {ticket.Customer}");
            }
            else if (_nextHandler != null)
            {
                Console.WriteLine($"Level 3 Support escalating ticket to management");
                _nextHandler.HandleTicket(ticket);
            }
        }
    }

    public class ManagementSupport : SupportHandler
    {
        public override void HandleTicket(SupportTicket ticket)
        {
            Console.WriteLine($"Management handling critical issue: {ticket.Issue} for {ticket.Customer}");
        }
    }
}
