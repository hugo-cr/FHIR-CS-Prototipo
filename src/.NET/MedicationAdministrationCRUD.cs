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
            string patientId
        ){

        }
    }
}