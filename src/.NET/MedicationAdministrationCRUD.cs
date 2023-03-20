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
        public static MedicationAdministration CreateMedicationAdministration(
            FhirClient fhirClient,
            string medicationId,
            string patientId,
            string patientName,
            string timestamp,
            //string medicationRequest,
            int ingestedDoseQuantity,
            string ingestedDoseUnit
        )
        {
            MedicationAdministration toCreate = new MedicationAdministration()
            {

                Status = MedicationAdministration.MedicationAdministrationStatusCodes.Completed, //estado de la administración de medicamento

                Medication = new ResourceReference() //medicacion administrada
                {
                    Reference = $"Medication/{medicationId}"
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
                //Effective = new FhirDateTime(1998,01,30,12,15,0, new TimeSpan()) //timestamp cuando se administró la medicina.
                Effective = new FhirDateTime(timestamp)
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
                         Value= ingestedDoseQuantity,
                         Unit= ingestedDoseUnit,
                         System= "http://unitsofmeasure.org",
                         Code= ingestedDoseUnit
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
            ResourceReference referencia_medicina = (ResourceReference) created.Medication;
            Medication nombre_medicina = MedicationCRUD.ReadMedication(fhirClient, referencia_medicina.Reference);
            //System.Console.WriteLine($"Creada la administración de medicina {created.Id} de nombre {nombre_medicina.Code.Coding[0].Display}");
            //System.Console.WriteLine($"Creada la administración de medicina {created.Id} de nombre testing");

            return created;

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
            //System.Console.WriteLine($"Borrando la administración de medicamento {id}");
            fhirClient.Delete($"MedicationAdministration/{id}");
        }

        public static MedicationAdministration ReadMedicationAdministration(
            FhirClient fhirClient,
            string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            return fhirClient.Read<MedicationAdministration>($"MedicationAdministration/{id}");
        }

        /// <summary>
        /// Consigue una lista de administraciones de medicación que coincida con los filtros entregados.
        /// </summary>
        /// <param name="fhirClient"></param>
        /// <param name="medicationAdmCriteria"></param>
        /// <param name="maxMedicationAdms"></param>
        /// <returns></returns>
        public static List<MedicationAdministration> GetMedicationAdministrations(
            FhirClient fhirClient,
            string[] medicationAdmCriteria = null,
            int maxMedicationAdms = 12 //Máximo de búsqueda.
        ){
            List<MedicationAdministration> medicationAdms = new List<MedicationAdministration>();
            Bundle medicationAdmBundle;

            if(medicationAdmCriteria == null || (medicationAdmCriteria.Length == 0))
            {
                medicationAdmBundle = fhirClient.Search<MedicationAdministration>(); //query REST de búsqueda por medicaciones  defecto
            }
            else
            {
                medicationAdmBundle = fhirClient.Search<Medication>(medicationAdmCriteria); //query REST de búsqueda por pacientes con nombre "test"
            }

            while(medicationAdmBundle != null)
            {
                //System.Console.WriteLine($"Total: {medicationAdmBundle.Total} Entry count: {medicationAdmBundle.Entry.Count}");

                foreach(Bundle.EntryComponent entry in medicationAdmBundle.Entry)
                {
                    if(entry.Resource != null) //Validación que recurso en bundle no sea nulo.
                    {
                        MedicationAdministration medicationAdm = (MedicationAdministration)entry.Resource;

                        medicationAdms.Add(medicationAdm);

                        
                        //System.Console.WriteLine($"- Entry: {medicationAdms.Count, 3} {entry.FullUrl}");
                        //System.Console.WriteLine($" -     IdMedicina: {medicationAdm.Id,20}");

                        // if(medicationAdm.Code.Coding.Count > 0 ) //Si hay códigos en la lista de códigos
                        // {
                        //     System.Console.WriteLine($" - NombreMedicina: {medication.Code.Coding[0].Display.ToString()}");
                        // }
                        
                    }

                    if(medicationAdms.Count >= maxMedicationAdms)
                    {
                        break;
                    }
                }

                if(medicationAdms.Count >= maxMedicationAdms)
                {
                    break;
                }
                //Obtener más resultados (paginación del Bundle)
                medicationAdmBundle= fhirClient.Continue(medicationAdmBundle);
            }
            return medicationAdms;
        }
    }

}


//RECORDAR VALIDAR FORMATO FHIR MEDIANTE ALGUNA HERRAMIENTA AUTOMATIZABLE 