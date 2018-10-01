using System;
using System.Collections.Generic;
using ColorSchemeManipulator.SchemeFormats.Handlers;

namespace ColorSchemeManipulator.SchemeFormats
{
    public class HandlerRegister<T>
    {
        private List<IColorFileHandler<T>> _handlers = new List<IColorFileHandler<T>>();
        
        public void Register(IColorFileHandler<T> handler)
        {
            _handlers.Add(handler);
        }

        public void Clear()
        {
            _handlers.Clear();
        }
        
        public IColorFileHandler<T> GetHandlerForFile(string sourceFile)
        {
            IColorFileHandler<T> result = null;
            bool oneFound = false;
            foreach (var handler in _handlers) {
                if (handler.Accepts(sourceFile)) {
                    if (oneFound) {
                        // multiple matches
                        throw new Exception("More than one possible handler");               
                    }
                    result = handler;
                    oneFound = true;
                }
            }

            return result;
        }
        
        
    }
}