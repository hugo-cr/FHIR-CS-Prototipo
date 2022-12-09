using System;
using System.Collections.Generic;
using Hl7.Fhir.Model; //Contiene los tipos de datos de los recursos FHIR.
using Hl7.Fhir.Rest; //Para llamadas HTTP y cliente FHIR.
using Hl7.Fhir.Serialization;

namespace fhir_cs_proto
{
    public static class PatientCRUD
    {
        //Crea un paciente con el nombre especificado.
        public static void CreatePatient(
            FhirClient fhirClient,
            string familyName,
            string givenName)
        {
            Patient toCreate = new Patient()
            {
                Name= new List<HumanName>()
                {
                    new HumanName()
                    {
                        Family = familyName,
                        Given = new List<string>()
                        {
                            givenName
                        }
                    }
                },
                BirthDateElement = new Date(1998, 01, 30)
            };

            Patient created= fhirClient.Create<Patient>(toCreate);
            System.Console.WriteLine($"Creado el paciente {created.Id}");
        }

        //Lee el paciente de la id especificada.
        public static Patient ReadPatient(
            FhirClient fhirClient,
            string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Patient patient= fhirClient.Read<Patient>($"Patient/{id}");
            return patient;
        }


        //Consigue una lista de pacientes que coincida con los filtros entregados.
        public static List<Patient> GetPatients(
            FhirClient fhirClient,
            string[] patientCriteria = null,
            int maxPatients = 12, //Máximo de búsqueda.
            bool onlyWithEncounters = false) //Flag para retornar sólo pacientes con encuentros.
        {
            List<Patient> patients = new List<Patient>();
            Bundle patientBundle;

            if(patientCriteria == null || (patientCriteria.Length == 0))
            {
                patientBundle = fhirClient.Search<Patient>(); //query REST de búsqueda por pacientes con nombre "test" 
            }
            else
            {
                patientBundle = fhirClient.Search<Patient>(patientCriteria); //query REST de búsqueda por pacientes con nombre "test"
            }

            while(patientBundle != null)
            {
                System.Console.WriteLine($"Total: {patientBundle.Total} Entry count: {patientBundle.Entry.Count}");

                foreach(Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    if(entry.Resource != null) //Validación que recurso en bundle no sea nulo.
                    {
                        Patient patient = (Patient)entry.Resource;

                        Bundle encounterBundle= fhirClient.Search<Encounter>( //Obtener los encuentros para un paciente del bundle.
                            new string[]
                            {
                                $"patient=Patient/{patient.Id}"
                            });
                        if (onlyWithEncounters && (encounterBundle.Total == 0))
                        {
                            continue;
                        }

                        patients.Add(patient);

                        
                        System.Console.WriteLine($"- Entry: {patients.Count, 3} {entry.FullUrl}");
                        System.Console.WriteLine($" -     IdPaciente: {patient.Id,20}");

                        if(patient.Name.Count > 0 ) //Si hay nombres en la lista de nombres
                        {
                            System.Console.WriteLine($" - NombrePaciente: {patient.Name[0].ToString()}");
                        }
                        
                        if(encounterBundle.Total > 0)
                        {
                            System.Console.WriteLine($" - Encounters totales: {encounterBundle.Total} Entry count: {encounterBundle.Entry.Count}");
                        }
                        
                    }

                    if(patients.Count >= maxPatients)
                    {
                        break;
                    }
                }

                if(patients.Count >= maxPatients)
                {
                    break;
                }
                //Obtener más resultados (paginación del Bundle)
                patientBundle= fhirClient.Continue(patientBundle);
            }
            return patients;
        }

        //Actualiza el paciente con más información.
        public static Patient UpdatePatient(
            FhirClient fhirClient,
            Patient patient)
        {
            patient.Telecom.Add(new ContactPoint()
            {
                System = ContactPoint.ContactPointSystem.Phone,
                Value = "993087485",
                Use = ContactPoint.ContactPointUse.Home
            }); //Agrega información de contacto, número de teléfono y uso.

            patient.Gender = AdministrativeGender.Male; //Agrega el género.

            return fhirClient.Update<Patient>(patient);
        }


        //Elimina un paciente con el id especificado.  
        public static void DeletePatient(
            FhirClient fhirClient,
            string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            System.Console.WriteLine($"Borrando el paciente {id}");
            fhirClient.Delete($"Patient/{id}");
        }
    }
}