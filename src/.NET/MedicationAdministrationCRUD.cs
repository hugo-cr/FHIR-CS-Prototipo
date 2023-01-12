using System;
using System.Collections.Generic;
using Hl7.Fhir.Model; //Contiene los tipos de datos de los recursos FHIR.
using Hl7.Fhir.Rest; //Para llamadas HTTP y cliente FHIR.
using Hl7.Fhir.Serialization;

namespace fhir_cs_proto
{
    public static class MedicationAdministrationCRUD
    {
        //Crea un nuevo recurso de tipo administración de medicamento, con el paciente y medicamento consumido.
        public static void CreateMedicationAdministration(
            FhirClient fhirClient,
            string medicationId,
            string patientId,
            string patientName,
            string timestamp,
            string medicationRequest,
            int ingestedDose
        )
        {
            MedicationAdministration toCreate = new MedicationAdministration()
            {

                Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed, //estado de la administración de medicamento

                Medication = new ResourceReference() //medicacion administrada
                {
                    Reference = $"#{medicationId}"
                },

                Subject = new ResourceReference() //paciente al cual se administró
                {
                    Reference = $"Patient/{patientId}",
                    Display = patientName
                },
                // Context= new ResourceReference() //Encuentro entre paciente y proveedor de salud donde se administró la medicación.
                // {
                //     Reference= "Encounter/1",
                //     Display= "encounter who leads to this prescription"
                // },
                Effective = new FhirDateTime(1998,01,30,12,15,0, new TimeSpan()) //timestamp cuando se administró la medicina.
                // {
                //     Value = timestamp
                // },
                ,
                //Performer No añadido en está ocasión
                // Request = new ResourceReference() //Receta/prescripción médica a la cual hace alusión esta ingesta.
                // {
                //     Reference = $"MedicationRequest/{medicationRequest}"
                // },
                //Device
                //Dosage= 
                Dosage= new MedicationAdministration.DosageComponent()
                {
                    Text= "0.05 - 0.1mg/kg IV over 2-5 minutes every 15 minutes as needed", //Instrucciones de médico en texto plano
                    Route= new CodeableConcept() //Parte del cuerpo a donde administrar.
                    {
                        Coding= new List<Coding>()
                        {
                            new Coding()
                            {
                                System= "http://snomed.info/sct",
                                Code= "255560000",
                                Display= "Intravenous"
                            }
                        }
                    },
                    Method= new CodeableConcept() //Técnica para administrar la medicación.
                    {
                        Coding= new List<Coding>()
                        {
                            new Coding()
                            {
                                System= "http://snomed.info/sct",
                                Code= "420620005",
                                Display= "Push - dosing instruction imperative (qualifier value)"
                            }
                        }
                    },
                    Dose= new Quantity() //Cantidad de medicamento por dosis (en este caso 7mg)
                    {
                         Value= ingestedDose,
                         Unit= "mg",
                         System= "http://unitsofmeasure.org",
                         Code= "mg"
                    },
                    Rate= new Quantity() //Cantidad de medicamento por unidad de tiempo (en este caso 4min).
                    {
                        Value= 4,
                        Unit= "min",
                        System= "http://unitsofmeasure.org",
                        Code= "min"
                    }
                } 
            };
            MedicationAdministration created= fhirClient.Create<MedicationAdministration>(toCreate);
            //string nombre_medicina = MedicationCRUD.ReadMedication(fhirClient, created.Medication);
            //System.Console.WriteLine($"Creada la administración de medicina {created.Id} de nombre {created.Medication.Reference}");
            //System.Console.WriteLine($"Creada la administración de medicina {created.Id} de nombre testing");

            //System.Console.WriteLine($"Creada la medicina {created.Id} de nombre {created.Code.Coding[0].Display}");
            //System.Console.WriteLine($"Creada la medicina {created.Id} de nombre {created.Code.Coding[0].Display}");
        }

        public static void DeleteMedicationAdministration(
            FhirClient fhirClient,
            string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            System.Console.WriteLine($"Borrando la administración de medicamento {id}");
            fhirClient.Delete($"MedicationAdministration/{id}");
        }
    }

}


//RECORDAR VALIDAR FORMATO FHIR MEDIANTE ALGUNA HERRAMIENTA AUTOMATIZABLE 