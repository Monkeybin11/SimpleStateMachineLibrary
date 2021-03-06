﻿using Microsoft.Extensions.Logging;
using SimpleStateMachineLibrary.Helpers;
using System;
using System.Collections.Generic;

namespace SimpleStateMachineLibrary
{
    public partial class State : NamedObject
    {
        private Action<State, Dictionary<string, object>> _onEntry;

        private Action<State, Dictionary<string, object>> _onExit;

        internal State(StateMachine stateMachine, string nameState, Action<State, Dictionary<string, object>> actionOnEntry, Action<State, Dictionary<string, object>> actionOnExit) : base(stateMachine, nameState)
        {
            stateMachine?._logger?.LogDebug("Create state \"{NameState}\" ", nameState);

            if (actionOnEntry != null)
            {
                OnEntry(actionOnEntry);
            }

            if (actionOnExit != null)
            {
                OnExit(actionOnExit);
            }

            StateMachine.AddState(this, out bool result, true);
        }

        //protected internal State(StateMachine stateMachine, string nameState) : base(stateMachine, nameState)
        //{
        //    stateMachine?._logger?.LogDebug("Create state \"{NameState}\" ", nameState);

        //    StateMachine.AddState(this, out bool result,  true);
           
        //}

        public State Delete()
        {
            return this.StateMachine.DeleteState(this);
        }

        public State TryDelete(out bool result)
        {
            return this.StateMachine.TryDeleteState(this, out result);
        }

        public State SetAsStartState()
        {
            this.StateMachine.SetStartState(this);
            return this;
        }

        public State OnEntry(Action<State, Dictionary<string, object>> actionOnEntry)
        {

            actionOnEntry = Check.Object(actionOnEntry, this.StateMachine?._logger);

            _onEntry += actionOnEntry;
            this.StateMachine._logger?.LogDebug("Method \"{NameMethod}\" subscribe on entry for state \"{NameState}\"", actionOnEntry.Method.Name, this.Name);
            return this;
        }

        public State OnExit(Action<State, Dictionary<string, object>> actionOnExit)
        {
            actionOnExit = Check.Object(actionOnExit, this.StateMachine?._logger);

            _onExit += actionOnExit;            
            this.StateMachine._logger?.LogDebug("Method \"{NameMethod}\" subscribe on exit for state \"{NameState}\"", actionOnExit.Method.Name, this.Name);
            return this;
        }

        internal void Entry(Dictionary<string, object> parameters)
        {
            _onEntry?.Invoke (this, parameters);
            this.StateMachine._logger?.LogDebugAndInformation("Entry to state \"{NameState}\"",  this.Name);
        }

        internal void Exit(Dictionary<string, object> parameters)
        {
            _onExit?.Invoke(this, parameters);
            this.StateMachine._logger?.LogDebugAndInformation("Exit from state \"{NameState}\"", this.Name);
        }
    }
}
