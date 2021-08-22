using Stateless;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Models.Entities
{
    public class Order
    {
        public Order()
        {
            StateMachineInit();
        }
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<LineItem> OrderItems { get; set; }
        public OrderState State { get; set; }
        public DateTime CreateDateUTC { get; set; }
        public string TransactionMetadata { get; set; }
        private StateMachine<OrderState, OrderStateTrigger> _machine;
        private void StateMachineInit()
        {
            _machine = new StateMachine<OrderState, OrderStateTrigger>(OrderState.Pending);
            _machine.Configure(OrderState.Pending)
                .Permit(OrderStateTrigger.PlaceOrder, OrderState.Processing)
                .Permit(OrderStateTrigger.Cancel, OrderState.Cancelled);

            _machine.Configure(OrderState.Processing)
                .Permit(OrderStateTrigger.Approve, OrderState.Completed)
                .Permit(OrderStateTrigger.Reject, OrderState.Declined);

            _machine.Configure(OrderState.Declined)
                .Permit(OrderStateTrigger.Approve, OrderState.Completed);


            _machine.Configure(OrderState.Completed)
                .Permit(OrderStateTrigger.Reject, OrderState.Refund);
        }
    }
}
