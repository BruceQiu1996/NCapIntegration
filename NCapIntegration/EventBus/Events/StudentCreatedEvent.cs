namespace NCapIntegration.EventBus.Events
{
    public class StudentCreatedEvent : EventBase<StudentCreatedEvent.StudentCreatedEventBody>
    {
        public class StudentCreatedEventBody
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
