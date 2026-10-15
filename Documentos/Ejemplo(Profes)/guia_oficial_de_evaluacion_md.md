Guía Oficial de Evaluación: Práctica Profesionalizante

1. Introducción y Marco Evaluativo
La presente guía establece las directrices institucionales y los estándares de Calidad Educativa para la evaluación final del "Proyecto Integrador" en la asignatura Práctica Profesionalizante. El propósito fundamental de esta instancia es validar la integración de competencias técnicas, arquitectónicas y profesionales mediante la entrega de una solución de software funcional y su respectiva validación ante un tribunal académico. El proceso evaluativo se rige por una escala estricta de 100 puntos, compuesta por la evaluación técnica del software y una defensa oral obligatoria. Esta normativa asegura que cada egresado cumpla con los estándares de estandarización académica exigidos para el ejercicio de la Ingeniería de Software.

2. Condiciones de Aprobación y Régimen de Promoción
El estado académico del estudiante se determinará de forma unívoca según el puntaje obtenido, conforme a la siguiente tabla de condiciones:

| Condición | Requisito de Puntaje | Consecuencia / Acción Requerida |
| --- | --- | --- |
| Promoción Directa | > 80 puntos | Exención total del examen final y acreditación directa de la asignatura. |
| Condición de Regularidad | < 80 puntos | El estudiante deberá rendir un Examen Final. Es requisito obligatorio realizar todas las refactorizaciones y modificaciones sobre el mismo proyecto original hasta alcanzar el estándar del 80%. |

3. Desglose de Criterios de Evaluación (Rúbrica Detallada)

3.1 Arquitectura y Diseño del Sistema (30 Puntos)
Se evaluará el cumplimiento riguroso de la estructura técnica sobre la cual se asienta la solución.
• Separación en 3 Capas (10 pts): Se exige el respeto estricto de la división funcional.
• Restricción Normativa: Se prohíbe terminantemente la inclusión de lógica de negocio en los controladores, así como la ejecución de consultas SQL directas o referencias a protocolos HTTP dentro de la capa de negocio.
• Uso de DTOS (10 pts): La API no deberá exponer las entidades de la base de datos bajo ninguna circunstancia. Es obligatorio el uso de Data Transfer Objects para el intercambio de información con el exterior.
• Base de Datos y ORM (10 pts): Diseño correcto del Modelo Entidad-Relación (DER) e implementación mediante Entity Framework Core, utilizando enfoques Code-First o Db-First de manera consistente.

3.2 Lógica de Negocio y Testing Unitario (30 Puntos)
Este eje constituye el núcleo de la evaluación y garantiza la robustez de la solución propuesta.
• Reglas de Negocio y Complejidad (15 pts): Implementación integral de las reglas base y los 4 niveles de Complejidad Extra estipulados en el catálogo de proyectos de la cátedra.
• Cobertura de Pruebas con xUnit (15 pts): Validación de la capa de Servicios mediante pruebas unitarias bajo el patrón AAA (Arrange, Act, Assert). Es imperativo el uso de Mocking (via Moq) para el aislamiento total de la persistencia.
• CLÁUSULA CRÍTICA: Todo proyecto que carezca de tests unitarios para la validación de sus reglas de negocio será reprobado automáticamente, sin derecho a ponderación de los demás ítems.

3.3 Calidad de Código y Buenas Prácticas (20 Puntos)
La madurez técnica se evaluará mediante el siguiente checklist de cumplimiento obligatorio: Manejo de Errores: Implementación de Status Codes correctos (200, 201, 400, 404, 409, 500).
• Gestión de Excepciones: Captura adecuada de errores, preferentemente centralizada mediante Middleware global.
• Clean Code: Uso de nombres descriptivos y consistencia en el lenguaje de programación elegido.
• Inyección de Dependencias: Configuración técnica correcta y desacoplada en el archivo Program.cs.
• Versionado con Git: Uso profesional del repositorio con commits lógicos e incrementales; se penalizará la entrega de un único commit final.

3.4 Despliegue Local y Defensa Técnica (20 Puntos)
Capacidad de entrega y justificación técnica de la solución.
• Entorno de Ejecución y Swagger (10 pts): El software deberá ser funcional "a la primera" en el entorno del evaluador. Es requisito que la interfaz de Swagger esté habilitada y configurada para la prueba inmediata de los endpoints.
• Defensa Técnica Oral (10 pts): Validación de la autoría y el conocimiento técnico exhaustivo del código fuente.

4. Protocolo de Defensa Oral y Entrega
Durante la defensa técnica, el evaluador someterá a los integrantes a un interrogatorio exhaustivo sobre las decisiones de diseño adoptadas. Es un requisito mandatario e ineludible que cada miembro del equipo sea capaz de explicar con precisión técnica el ciclo de vida completo de la petición (Request/Response) dentro de su arquitectura. "La autonomía técnica es un estándar no negociable: la calificación es colectiva pero la responsabilidad es individual. Todos los miembros del equipo deben poseer un dominio total del código fuente, siendo capaces de justificar cada línea de código y responder preguntas conceptuales. Un integrante que no demuestre conocimiento de la solución compromete la evaluación de todo el equipo."

5. Resumen de Pesos Evaluativos

| Criterio | Puntaje Máximo |
| --- | --- |
| Arquitectura y Diseño del Sistema | 30 pts |
| Lógica de Negocio y Testing Unitario | 30 pts |
| Calidad de Código y Buenas Prácticas | 20 pts |
| Despliegue Local y Defensa Técnica | 20 pts |
| TOTAL DE EVALUACIÓN | 100 pts |