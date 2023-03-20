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
            var settings = new FhirClientSettings
            {
                PreferredFormat = ResourceFormat.Json,
                PreferredReturn = Prefer.ReturnRepresentation
            };

            FhirClient fhirClient = new FhirClient(_fhirServer, settings); //Cliente que establece conexión

            UserInputProgram.InputLoop(fhirClient);

            return 0;
        }
        //Ingreso de recurso de administración de medicación.
        //MedicationAdministrationCRUD.CreateMedicationAdministration(fhirClient, medications[0].Id, patients[0].Id, patients[0].Name[0].ToString(), DateTime.Now.ToString(), "1", 7);


        //Crear paciente y medicina
        //PatientCRUD.CreatePatient(fhirClient, "Perez", "Juanito");
        //MedicationCRUD.CreateMedication(fhirClient, "test", "test");


        //Borrar paciente y/o medicina específico
        //MedicationCRUD.DeleteMedication(fhirClient, "54");


        //Obtener lista de pacientes y medicinas
        //List<Patient> patients= PatientCRUD.GetPatients(fhirClient);
        //List<Medication> medications = MedicationCRUD.GetMedications(fhirClient, null);

        //Conteo de pacientes y medicinas obtenidos.
        //System.Console.WriteLine($"Se han encontrado: {patients.Count} pacientes!");     
        //System.Console.WriteLine($"Se han encontrado: {medications.Count} medicinas!");


        //Borrado de recurso de administración de medicación
        //MedicationAdministrationCRUD.DeleteMedicationAdministration(fhirClient, "1703");

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
        //return 0;

    }
    public static class UserInputProgram
    {
        public static void InputLoop(FhirClient fhirClient)
        {   
            string userInput;
            string patientId = "1";
            string patientName = "Hugo Contreras";
            while(true)
            {
                System.Console.WriteLine("Ingrese CRUD de recursos a operar: ");
                System.Console.WriteLine("1 - Medicamentos");
                System.Console.WriteLine("2 - Ingesta de medicamentos");
                System.Console.WriteLine("3 - Ver información de paciente");
                System.Console.WriteLine("4 - Salir");

                userInput = Console.ReadLine();
                System.Console.WriteLine();
                switch(userInput)
                {
                    case "1":
                        while(userInput != "4")
                        {
                            System.Console.WriteLine("CRUD Medicamentos - ¿Qué desea realizar?");
                            System.Console.WriteLine("1 - Ingresar medicamento");
                            System.Console.WriteLine("2 - Buscar medicamento");
                            System.Console.WriteLine("3 - Borrar medicamento");
                            System.Console.WriteLine("4 - Volver atrás");
                            userInput = Console.ReadLine();
                            switch(userInput)
                            {
                                case "1":
                                    System.Console.WriteLine("Aún viendo cómo se usará el sistema de códigos para gestionar medicamentos, ejemplos en documentación usan sistema de SNOMED.");
                                    break;
                                case "2":
                                    System.Console.WriteLine();
                                    System.Console.WriteLine("Ingrese id de medicamento a buscar.");
                                    string medId = Console.ReadLine();
                                    Medication toSearch = MedicationCRUD.ReadMedication(fhirClient, medId);

                                    while(userInput != "5")
                                    {
                                        System.Console.WriteLine();
                                        System.Console.WriteLine($"¿Qué desea consultar sobre el medicamento de id {toSearch.Id}?");
                                        System.Console.WriteLine("1 - Consultar nombre de medicamento");
                                        System.Console.WriteLine("2 - Consultar formato de medicamento");
                                        System.Console.WriteLine("3 - Consultar ingredientes de medicamento");
                                        System.Console.WriteLine("4 - Consultar cantidad de medicamento en paquete");
                                        System.Console.WriteLine("5 - Volver atrás");
                                        userInput = Console.ReadLine();
                                        switch(userInput){
                                            case "1":
                                                System.Console.WriteLine($"{toSearch.Code.Coding[0].Display}");
                                                break;
                                            case "2":
                                                System.Console.WriteLine($"{toSearch.Form.Coding[0].Display}");
                                                break;
                                            case "3":
                                                Medication.IngredientComponent medIngr = toSearch.Ingredient[0];
                                                CodeableConcept medItem = (CodeableConcept)medIngr.Item;
                                                Console.WriteLine($"{medItem.Coding[0].Display} {medIngr.Strength.Numerator.Value}{medIngr.Strength.Numerator.Code}/{medIngr.Strength.Denominator.Value}{medIngr.Strength.Denominator.Code}");
                                                break;
                                            case "4":
                                                Ratio medAmount = toSearch.Amount;
                                                System.Console.WriteLine($"{medAmount.Numerator.Value}{medAmount.Numerator.Code}/{medAmount.Denominator.Code}");
                                                break;
                                            case "5":
                                                break;
                                            default:
                                                System.Console.WriteLine("Opción inválida. Intente nuevamente.");
                                                break;

                                        }
                                    }
                                    break;
                                case "3":
                                    System.Console.WriteLine();
                                    System.Console.WriteLine("Ingrese id de medicamento a borrar.");
                                    medId = Console.ReadLine();
                                    MedicationCRUD.DeleteMedication(fhirClient, medId);
                                    System.Console.WriteLine($"Medicamento de id {medId} borrada exitosamente!");
                                    break;
                                case "4":
                                    break;
                                default:
                                    System.Console.WriteLine("Opción inválida. Intente nuevamente.");
                                    break;
                            }
                            System.Console.WriteLine();
                        }
                        break;
                    case "2":
                        while(userInput != "5")
                        {
                            System.Console.WriteLine("CRUD Ingesta de medicamentos - ¿Qué desea realizar?");
                            System.Console.WriteLine("1 - Ingresar ingesta de medicamento");
                            System.Console.WriteLine("2 - Buscar ingesta de medicamento");
                            System.Console.WriteLine("3 - Mostrar lista completa de ingestas");
                            System.Console.WriteLine("4 - Borrar ingesta de medicamento");
                            System.Console.WriteLine("5 - Volver atrás");
                            userInput = Console.ReadLine();
                            switch(userInput)
                            {
                                case "1":
                                    System.Console.WriteLine();
                                    System.Console.WriteLine("Ingrese id de medicamento ingerido");
                                    string medId = Console.ReadLine();
                                    System.Console.WriteLine("Ingrese dosis de medicamento");
                                    int doseQuantity = Convert.ToInt32(Console.ReadLine());
                                    System.Console.WriteLine("Ingrese unidad de medida de dosis");
                                    string doseUnit = Console.ReadLine();

                                    //Get current timestamp
                                    DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
                                    string timestamp = dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");

                                    MedicationAdministration created= MedicationAdministrationCRUD.CreateMedicationAdministration(fhirClient, medId, patientId, 
                                                                                                patientName, timestamp, 
                                                                                                doseQuantity, doseUnit);
                                    System.Console.WriteLine($"Ingesta de medicamento de id: {created.Id} creada exitosamente!");
                                    break;
                                case "2":
                                    System.Console.WriteLine();
                                    System.Console.WriteLine("Ingrese id de ingesta de medicamento a buscar.");
                                    string medAdmId = Console.ReadLine();
                                    MedicationAdministration toSearch = MedicationAdministrationCRUD.ReadMedicationAdministration(fhirClient, medAdmId);
                                    
                                    while(userInput != "5"){
                                        System.Console.WriteLine();
                                        System.Console.WriteLine($"¿Qué desea consultar sobre la ingesta de medicamento de id {toSearch.Id}?");
                                        System.Console.WriteLine("1 - Consultar medicamento ingerido");
                                        System.Console.WriteLine("2 - Consultar dosis de ingesta");
                                        System.Console.WriteLine("3 - Consultar hora de ingesta");
                                        System.Console.WriteLine("4 - Consultar fecha de ingesta");
                                        System.Console.WriteLine("5 - Volver atrás");
                                        userInput = Console.ReadLine();
                                        switch(userInput)
                                        {
                                            case "1":
                                                ResourceReference medRef = (ResourceReference)toSearch.Medication;
                                                Medication medRequested = MedicationCRUD.ReadMedication(fhirClient, medRef.Reference);
                                                System.Console.WriteLine($"{medRequested.Code.Coding[0].Display}");
                                                break;
                                            case "2":
                                                System.Console.WriteLine($"{toSearch.Dosage.Dose.Value} {toSearch.Dosage.Dose.Unit}.");
                                                break;
                                            case "3":
                                                System.Console.WriteLine($"El medicamento fue ingerido a las {toSearch.Effective.ToString().Substring(11,8)}.");
                                                break;
                                            case "4":
                                                DateTime medAdmDate= DateTime.ParseExact(toSearch.Effective.ToString().Substring(0,10),"yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                                                string formattedDate = medAdmDate.ToString("dd-MM-yyyy");
                                                System.Console.WriteLine($"El medicamento fue ingerido el día {formattedDate}.");
                                                break;
                                            case "5":
                                                break;
                                            default:
                                                System.Console.WriteLine("Opción inválida. Intente nuevamente.");
                                                break;
                                        }
                                    }
                                    break;
                                case "3":
                                    System.Console.WriteLine();
                                    List<MedicationAdministration> medAdmList=  MedicationAdministrationCRUD.GetMedicationAdministrations(fhirClient);
                                    int counter = 1;
                                    foreach(MedicationAdministration currMedAdm in medAdmList)
                                    {
                                        ResourceReference medRef = (ResourceReference)currMedAdm.Medication;
                                        Medication medRequested = MedicationCRUD.ReadMedication(fhirClient, medRef.Reference);
                                        DateTime medAdmDate= DateTime.ParseExact(currMedAdm.Effective.ToString().Substring(0,10),"yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                                        string formattedDate = medAdmDate.ToString("dd-MM-yyyy");                                                
                                        System.Console.WriteLine($"{counter} - Id: {currMedAdm.Id} , {currMedAdm.Dosage.Dose.Value} {currMedAdm.Dosage.Dose.Unit} de {medRequested.Code.Coding[0].Display} ingerido el {formattedDate} a las {currMedAdm.Effective.ToString().Substring(11,8)} ");
                                        counter++;
                                    }
                                    break;
                                case "4":
                                    System.Console.WriteLine();
                                    System.Console.WriteLine("Ingrese id de ingesta de medicamento a borrar.");
                                    medAdmId = Console.ReadLine();
                                    MedicationAdministrationCRUD.DeleteMedicationAdministration(fhirClient, medAdmId);
                                    System.Console.WriteLine($"Ingesta de medicamento de id {medAdmId} borrada exitosamente!");
                                    break;
                                case "5":
                                    break;
                                default:
                                    System.Console.WriteLine("Opción inválida. Intente nuevamente.");
                                    break;
                            }
                            System.Console.WriteLine();
                        }
                        break;
                    case "3":
                        System.Console.WriteLine($"Paciente Actual: {patientName} de id {patientId}.");
                        System.Console.WriteLine();
                        break;
                    case "4":
                        System.Console.WriteLine("Programa terminado exitosamente.");
                        return;
                    default:
                        System.Console.WriteLine("Opción inválida. Intente nuevamente.");
                        System.Console.WriteLine();
                        break;
                }
            }
        }
    }
}

