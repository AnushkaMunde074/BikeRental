import { useState, useCallback, useRef, useEffect } from 'react';

export function useToast(duration = 4000) {
  const [toast, setToast] = useState('');
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    return () => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
    };
  }, []);

  const showToast = useCallback(
    (msg: string) => {
      if (timeoutRef.current) {
        clearTimeout(timeoutRef.current);
      }
      setToast(msg);
      timeoutRef.current = setTimeout(() => setToast(''), duration);
    },
    [duration],
  );

  return { toast, showToast };
}
