namespace NCapIntegration.EventBus.Events
{
    public class StudentDeletedEvent : EventBase<StudentDeletedEvent.StudentDeletedEventBody>
    {
        public class StudentDeletedEventBody
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
