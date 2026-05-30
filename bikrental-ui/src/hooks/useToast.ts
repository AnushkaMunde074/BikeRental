import { useState, useCallback } from 'react';

export function useToast(duration = 4000) {
  const [toast, setToast] = useState('');

  const showToast = useCallback(
    (msg: string) => {
      setToast(msg);
      setTimeout(() => setToast(''), duration);
    },
    [duration],
  );

  return { toast, showToast };
}
