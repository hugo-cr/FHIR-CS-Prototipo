using System;
using System.Collections.Generic;
using Hl7.Fhir.Model; //Contiene los tipos de datos de los recursos FHIR.
using Hl7.Fhir.Rest; //Para llamadas HTTP y cliente FHIR.
using Hl7.Fhir.Serialization;

namespace fhir_cs_proto
{
    public static class Program
    {
        

        private static readonly Dictionary<string,string> _fhirServers = new Dictionary<string, string>()
        {
            {"PublicVonk", "http://vonk.fire.ly"},
            {"PublicHAPI", "http://hapi.fhir.org/baseR4/"},
            {"Local", "http://localhost:8080/fhir"}
        };        
        private static readonly string _fhirServer =  _fhirServers["Local"];
        
        //Main
        static int Main(string[] args)
        {
            var settings= new FhirClientSettings{
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation
            };

            FhirClient fhirClient = new FhirClient(_fhirServer, settings); //Cliente que establece conexión

            //Crear paciente y medicina
            //PatientCRUD.CreatePatient(fhirClient, "Perez", "Juanito");
            //MedicationCRUD.CreateMedication(fhirClient, "test", "test");


            //Borrar paciente y/o medicina específico
            //MedicationCRUD.DeleteMedication(fhirClient, "54");
            //Obtener lista de pacientes y medicinas
            List<Patient> patients= PatientCRUD.GetPatients(fhirClient);
            List<Medication> medications = MedicationCRUD.GetMedications(fhirClient, null);

            //Conteo de pacientes y medicinas obtenidos.
            System.Console.WriteLine($"Se han encontrado: {patients.Count} pacientes!");     
            System.Console.WriteLine($"Se han encontrado: {medications.Count} medicinas!");


            //Ingreso de recurso de administración de medicación.
            //MedicationAdministrationCRUD.CreateMedicationAdministration
            //(fhirClient, medications[0].Id, patients[0].Id, patients[0].Name[0].ToString(), DateTime.Now.ToString(), "1", 7);

            //System.Console.WriteLine($"el div de medicina contiene: {medications[0].Id} y  {medications[0].Text.Div.ToString()} y {medications[0].Text.Status.ToString()}");
            //string firstId= null; //Para guardar el primer id.
            /*
            foreach(Patient patient in patients)
            {
                if(string.IsNullOrEmpty(firstId))
                {
                    firstId= patient.Id;
                    continue;
                }
                DeletePatient(fhirClient, patient.Id);  //Elimina todos los pacientes excepto el primero.
            }
    
            Patient firstPatient = ReadPatient(fhirClient, firstId);

            System.Console.WriteLine($"Nombre de paciente leído: {firstPatient.Name[0].ToString()}");
            /*
            Patient updatedPatient= UpdatePatient(fhirClient, firstPatient);
            Patient readFinal = ReadPatient(fhirClient, firstId);
            */
            return 0;
        }
 
    }
}

