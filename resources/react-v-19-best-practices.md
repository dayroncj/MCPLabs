React 19 introduce cambios significativos que simplifican la gestión de estados, formularios y renderizado. A continuación, un resumen de las mejores prácticas y los nuevos patrones que definen esta versión.

1. **Gestión de Formularios con "Actions"**

   La mayor evolución en React 19 es el concepto de Actions. Ya no es necesario gestionar manualmente estados de carga (pending), error o éxito mediante múltiples useState.

   - **Usa `useActionState`:** Este hook es el estándar para manejar formularios. Retorna el estado de la acción, el "trigger" para el formulario y un indicador de pendiente.

   - **Transiciones implícitas:** Al usar el atributo `action` en un `<form>` React inicia automáticamente una transición, manteniendo la interfaz interactiva mientras se procesa la respuesta.

   ```
      // Patrón recomendado en React 19
      const [error, submitAction, isPending] = useActionState(async (previousState, formData) => {
       const error = await updateProfile(formData);
        if (error) return error;
        return null;
      }, null);
      
      return (
        <form action={submitAction}>
          <input name="name" />
          <button disabled={isPending}>Actualizar</button>
        </form>
      );
      ```

2. **Optimización con el Compilador de React**

   React 19 se apoya en el nuevo React Compiler (en entornos que ya lo tienen habilitado).

    - **Adiós a memo, useMemo y useCallback:** La recomendación actual es dejar de usarlos preventivamente. El compilador detecta automáticamente qué partes del árbol de componentes necesitan re-renderizarse. Solo usarlos si hay un caso de borde extremadamente complejo donde el compilador necesite ayuda manual.

    - **Código más limpio:** Escribir lógica de negocio legible en lugar de envolver cada función en hooks de optimización.

3. **Manejo de Datos Asíncronos con use**

   El nuevo API use permite leer recursos (como Promesas o Contextos) directamente en el cuerpo del renderizado.

   - **Carga condicional:** A diferencia de los hooks tradicionales, use puede llamarse dentro de condicionales o bucles.

   - **Integración con Suspense:** Cuando se usa use(promise), React suspende el componente automáticamente hasta que la promesa se resuelve.

4. **Mejoras en la Arquitectura de Componentes**

    - **Ref como Prop:** Ya no es necesario usar forwardRef. En React 19, ref es una prop normal. Pasarla directamente a los componentes funcionales.

    - **Metadatos Nativos:** Ahora es posible renderizar etiquetas `<title>`, `<meta>` y `<link>` en cualquier componente. React las "elevará" (hoisting) automáticamente al `<head>` del documento, facilitando el SEO en aplicaciones tipo SPA o SSR.

5. **Feedback Instantáneo: Optimistic Updates**

   Para mejorar la experiencia de usuario (UX), React 19 introduce el hook useOptimistic.

    - **Estado temporal:** Permite mostrar un resultado en la interfaz antes de que la operación del servidor termine.

    - **Sincronización automática:** Si la operación falla, React revierte el estado al valor real del servidor sin intervención manual.

|Antes|React 19|
|---|---|
|`useEffect` para cargar datos al montar|API `use` + `Suspense`|
|`useState` para `isLoading` en formularios|`useActionState` (isPending)|
|`forwardRef(Component)`|`const Component = ({ ref, ...props })`|
|Manual Optimistic UI (logica compleja)|`Hook useOptimistic`|