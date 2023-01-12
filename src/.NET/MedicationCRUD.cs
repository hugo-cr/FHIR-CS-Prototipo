using System;
using System.Collections.Generic;
using Hl7.Fhir.Model; //Contiene los tipos de datos de los recursos FHIR.
using Hl7.Fhir.Rest; //Para llamadas HTTP y cliente FHIR.
using Hl7.Fhir.Serialization;

namespace fhir_cs_proto
{
    public static class MedicationCRUD
    {
        /// <summary>
        /// Crea una medicina con los datos especificados.
        /// </summary>
        /// <param name="fhirClient"></param>
        /// <param name="medicationName"></param>
        /// <param name="medicationCode"></param>
        public static void CreateMedication(
            FhirClient fhirClient,
            string medicationName= "Lorazepam 2mg/ml injection solution 1ml vial (product)",
            string medicationCode= "400621001"
        ){
            Medication toCreate = new Medication()
            {
                // Text = new Narrative()
                // {
                //     Status = Narrative.NarrativeStatus.Generated,
                //     Div = "<div xmlns=\"http://www.w3.org/1999/xhtml\"><p><b>Generated Narrative: Medication</b><a name=\"med0313\"> </a></p><"
                // },
                Code= new CodeableConcept() //código para identificar medicina
                {
                    Coding= new List<Coding>()
                    {
                        new Coding()
                        {
                            System= "http://snomed.info/sct",
                            Code= medicationCode,
                            Display= medicationName
                        }
                    }
                },
                Form= new CodeableConcept() //Forma de dosis
                {
                    Coding= new List<Coding>()
                    {
                        new Coding(){
                            System= "http://snomed.info/sct",
                            Code= "385219001",
                            Display= "Injection solution (qualifier value)"
                        }
                    },
                },
                Amount= new Ratio() //Cantidad medicamento
                {
                    Numerator= new Quantity()
                    {
                        Value= 1,
                        Unit= "ml",
                        System= "http://unitsofmeasure.org",
                        Code= "ml"
                    },
                    Denominator= new Quantity()
                    {
                        Value= 1,
                        System= "http://terminology.hl7.org/CodeSystem/medicationknowledge-package-type",
                        Code= "vial"
                    }
                },
                Ingredient= new List<Medication.IngredientComponent>(){
                    new Medication.IngredientComponent()
                    {
                        Item= new CodeableConcept()
                        {
                            Coding= new List<Coding>()
                            {
                                new Coding()
                                {
                                    System= "http://snomed.info/sct",
                                    Code= "387106007",
                                    Display= "Lorazepam (substance)"
                                }
                            }
                        },
                        Strength= new Ratio()
                        {
                            Numerator= new Quantity()
                            {
                                Value= 2,
                                System= "http://unitsofmeasure.org",
                                Code= "mg"
                            },
                            Denominator= new Quantity()
                            {
                                Value= 1,
                                System= "http://unitsofmeasure.org",
                                Code= "mL"
                            }
                        }
                    }
                }
            };

            Medication created= fhirClient.Create<Medication>(toCreate);
            System.Console.WriteLine($"Creada la medicina {created.Id} de nombre {created.Code.Coding[0].Display}");
        }
        
        /// <summary>
        /// //Lee el paciente de la id especificada.
        /// </summary>
        /// <param name="fhirClient"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>/
        public static Medication ReadMedication(
            FhirClient fhirClient,
            string id
        ){
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Medication medication= fhirClient.Read<Medication>($"Medication/{id}");
            return medication;
        }

        /// <summary>
        /// Consigue una lista de medicaciones que coincida con los filtros entregados.
        /// </summary>
        /// <param name="fhirClient"></param>
        /// <param name="medicationCriteria"></param>
        /// <param name="maxMedications"></param>
        /// <returns></returns>
        public static List<Medication> GetMedications(
            FhirClient fhirClient,
            string[] medicationCriteria = null,
            int maxMedications = 12 //Máximo de búsqueda.
        ){
            List<Medication> medications = new List<Medication>();
            Bundle medicationBundle;

            if(medicationCriteria == null || (medicationCriteria.Length == 0))
            {
                medicationBundle = fhirClient.Search<Medication>(); //query REST de búsqueda por medicaciones  defecto
            }
            else
            {
                medicationBundle = fhirClient.Search<Medication>(medicationCriteria); //query REST de búsqueda por pacientes con nombre "test"
            }

            while(medicationBundle != null)
            {
                System.Console.WriteLine($"Total: {medicationBundle.Total} Entry count: {medicationBundle.Entry.Count}");

                foreach(Bundle.EntryComponent entry in medicationBundle.Entry)
                {
                    if(entry.Resource != null) //Validación que recurso en bundle no sea nulo.
                    {
                        Medication medication = (Medication)entry.Resource;

                        medications.Add(medication);

                        
                        System.Console.WriteLine($"- Entry: {medications.Count, 3} {entry.FullUrl}");
                        System.Console.WriteLine($" -     IdMedicina: {medication.Id,20}");

                        if(medication.Code.Coding.Count > 0 ) //Si hay códigos en la lista de códigos
                        {
                            System.Console.WriteLine($" - NombreMedicina: {medication.Code.Coding[0].Display.ToString()}");
                        }
                        
                    }

                    if(medications.Count >= maxMedications)
                    {
                        break;
                    }
                }

                if(medications.Count >= maxMedications)
                {
                    break;
                }
                //Obtener más resultados (paginación del Bundle)
                medicationBundle= fhirClient.Continue(medicationBundle);
            }
            return medications;
        }

        /// <summary>
        /// Elimina una medicina con el id especificado.  
        /// </summary>
        /// <param name="fhirClient"></param>
        /// <param name="id"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void DeleteMedication(
            FhirClient fhirClient,
            string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            System.Console.WriteLine($"Borrando la medicina {id}");
            fhirClient.Delete($"Medication/{id}");
        }
    }
    
}