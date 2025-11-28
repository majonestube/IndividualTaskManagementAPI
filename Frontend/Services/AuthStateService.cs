// Services/AuthStateService.cs
using System;

namespace Frontend.Services
{
    public class AuthStateService
    {
        public event Action<Type>? LayoutChanged;

        public void SetLayout(Type layout)
        {
            LayoutChanged?.Invoke(layout);
        }
    }
}