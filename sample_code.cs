using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthCareManagement
{
    // Data Model
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Doctor { get; set; }
        public string Department { get; set; }
        public string Notes { get; set; }
    }

    // Repository Interface
    public interface IPatientRepository
    {
        void AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        Patient GetPatientById(int id);
        IEnumerable<Patient> GetAllPatients();
        void AddAppointment(int patientId, Appointment appointment);
        IEnumerable<Appointment> GetAppointmentsByPatientId(int patientId);
    }

    // In-Memory Repository Implementation
    public class PatientRepository : IPatientRepository
    {
        private readonly List<Patient> _patients = new List<Patient>();

        public void AddPatient(Patient patient)
        {
            patient.Id = _patients.Any() ? _patients.Max(p => p.Id) + 1 : 1;
            _patients.Add(patient);
        }

        public void UpdatePatient(Patient patient)
        {
            var existingPatient = GetPatientById(patient.Id);
            if (existingPatient != null)
            {
                existingPatient.FirstName = patient.FirstName;
                existingPatient.LastName = patient.LastName;
                existingPatient.DateOfBirth = patient.DateOfBirth;
                existingPatient.Gender = patient.Gender;
                existingPatient.Address = patient.Address;
                existingPatient.PhoneNumber = patient.PhoneNumber;
            }
        }

        public Patient GetPatientById(int id)
        {
            return _patients.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return _patients;
        }

        public void AddAppointment(int patientId, Appointment appointment)
        {
            var patient = GetPatientById(patientId);
            if (patient != null)
            {
                appointment.Id = patient.Appointments.Any() ? patient.Appointments.Max(a => a.Id) + 1 : 1;
                patient.Appointments.Add(appointment);
            }
        }

        public IEnumerable<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            var patient = GetPatientById(patientId);
            return patient?.Appointments ?? Enumerable.Empty<Appointment>();
        }
    }

    // Service Layer
    public class PatientService
    {
        private readonly IPatientRepository _repository;

        public PatientService(IPatientRepository repository)
        {
            _repository = repository;
        }

        public void RegisterPatient(Patient patient)
        {
            _repository.AddPatient(patient);
        }

        public void UpdatePatientDetails(Patient patient)
        {
            _repository.UpdatePatient(patient);
        }

        public Patient GetPatientInformation(int patientId)
        {
            return _repository.GetPatientById(patientId);
        }

        public IEnumerable<Patient> GetAllPatients()
        {
            return _repository.GetAllPatients();
        }

        public void ScheduleAppointment(int patientId, Appointment appointment)
        {
            _repository.AddAppointment(patientId, appointment);
        }

        public IEnumerable<Appointment> GetPatientAppointments(int patientId)
        {
            return _repository.GetAppointmentsByPatientId(patientId);
        }
    }

    // Example Usage
    class Program
    {
        static void Main(string[] args)
        {
            IPatientRepository repository = new PatientRepository();
            PatientService service = new PatientService(repository);

            // Register a new patient
            var patient = new Patient
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateTime(1985, 5, 20),
                Gender = "Male",
                Address = "123 Main St",
                PhoneNumber = "555-1234"
            };
            service.RegisterPatient(patient);

            // Update patient details
            patient.Address = "456 Oak St";
            service.UpdatePatientDetails(patient);

            // Schedule an appointment
            var appointment = new Appointment
            {
                Date = new DateTime(2024, 10, 1, 14, 0, 0),
                Doctor = "Dr. Smith",
                Department = "Cardiology",
                Notes = "Regular check-up"
            };
            service.ScheduleAppointment(patient.Id, appointment);

            // Retrieve patient information
            var retrievedPatient = service.GetPatientInformation(patient.Id);
            Console.WriteLine($"Patient: {retrievedPatient.FirstName} {retrievedPatient.LastName}, Address: {retrievedPatient.Address}");

            // List all appointments
            var appointments = service.GetPatientAppointments(patient.Id);
            foreach (var app in appointments)
            {
                Console.WriteLine($"Appointment with {app.Doctor} on {app.Date}, Department: {app.Department}");
            }
        }
    }
}
