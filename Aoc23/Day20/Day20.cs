using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc23
{
    public class Day20 : AllDays
    {
        public Day20() : base("Day20")
        {
        }

        public override void ExecutePart1()
        {
            var modules = ParseInput();

            var pulseCounter = new PulseCounter();
            var pulseQueue = new Queue<Pulse>();
            pulseQueue.Enqueue(new Pulse { Module = modules["broadcaster"], IsHighPulse = false });
            for (int i = 0; i < 1000; i++)
            {
                pulseCounter.LowPulses++;
                while (pulseQueue.Count > 0)
                {
                    var pulse = pulseQueue.Dequeue();
                    pulse.Module.ReceivePulse(pulse.Module.Name, pulse.IsHighPulse, pulseCounter, pulseQueue);
                }

                if (i < 999)
                {
                    pulseQueue.Enqueue(new Pulse { Module = modules["broadcaster"], IsHighPulse = false });
                }
                //Console.WriteLine($"Low Pulses: {pulseCounter.LowPulses}, High Pulses: {pulseCounter.HighPulses}");
            }


            Console.WriteLine($"Low Pulses: {pulseCounter.LowPulses}, High Pulses: {pulseCounter.HighPulses}");
            Console.WriteLine($"Multiplication Result: {pulseCounter.LowPulses * pulseCounter.HighPulses}");
        }

        private Dictionary<string, Module> ParseInput()
        {
            var modules = new Dictionary<string, Module>();
            foreach (var line in Lines)
            {
                var parts = line.Trim().Split(" -> ");
                var moduleInfo = parts[0].Trim().ToCharArray();
                var moduleName = parts[0].Trim();
                var moduleType = moduleInfo[0];

                if (moduleType == '%' || moduleType == '&')
                {
                    moduleName = new string(moduleInfo[1..]);
                }

                Module module;
                if (moduleType == '%') module = new FlipFlopModule(moduleName, parts[1]);
                else if (moduleType == '&') module = new ConjunctionModule(moduleName, parts[1]);
                else module = new BroadcasterModule(moduleName, parts[1]);

                modules[moduleName] = module;
            }

            var untypes = new List<(string, Module, string)>();
            foreach (var module in modules)
            {
                foreach (var outputName in module.Value.StringOutput.Split(","))
                {
                    if (string.IsNullOrEmpty(outputName)) continue;

                    var name = outputName.Trim();
                    if (!modules.ContainsKey(name))
                    {
                        var m = new UnTypeModule(name, "");

                        untypes.Add((name, m, module.Key));
                        // modules[name] = m;
                    }
                    else
                    {
                        module.Value.Outputs.Add(modules[name]);
                        if (modules[name].Type == "Conjunction")
                        {
                            (modules[name] as ConjunctionModule).inputModules.Add(module.Key, false);
                        }
                    }

                }
            }

            foreach (var untype in untypes)
            {
                modules[untype.Item1] = untype.Item2;
                modules[untype.Item3].Outputs.Add(modules[untype.Item1]);
            }

            return modules;
        }

        public override void ExecutePart2()
        {
            var modules = ParseInput();

            //Dictionary<string, List<long>> results = new Dictionary<string, List<long>>();

            int iteration = 0;
            while (iteration >=0) // don't run it, run with breakdown
            {
                var pulseCounter = new PulseCounter();
                var pulseQueue = new Queue<Pulse>();
                pulseQueue.Enqueue(new Pulse { Module = modules["broadcaster"], IsHighPulse = false, Caller = "Button" });
                //for (int i = 0; ; i++)
                //{
                pulseCounter.LowPulses++;
                while (pulseQueue.Count > 0)
                {
                    var pulse = pulseQueue.Dequeue();

                    pulse.Module.ReceivePulse(pulse.Module.Name, pulse.IsHighPulse, pulseCounter, pulseQueue);
                    if (pulse.Module.Name == "dn" && pulse.IsHighPulse)
                    {

                        Console.WriteLine($"{pulse.Caller} : {iteration}");
                        //var xx = (pulse.Module as ConjunctionModule).inputModules.Values.All(c => c);
                        
                    }
                }


                // pulseQueue.Enqueue(new Pulse { Module = modules["broadcaster"], IsHighPulse = false, Caller = "Button" });

                //}
            }

            //
            var lcmlist = new List<long>()
            {
                8005 - 4002, 8053-4026 , 7837-3918 , 7833-3916
            };
            Console.WriteLine("Solution  : " + CalculateLCM(lcmlist));


        }

        class Pulse
        {
            public Module Module { get; set; }
            public bool IsHighPulse { get; set; }

            public string Caller { get; set; }
        }

        abstract class Module
        {
            public string Name { get; set; }
            public List<Module> Outputs { get; set; } = new List<Module>();

            public string Type { get; set; }
            public string StringOutput { get; set; }

            public abstract void ReceivePulse(string sender, bool isHighPulse, PulseCounter pulseCounter, Queue<Pulse> pulseQueue);

            public abstract void PulseAction(string sender, bool isHighPulse, PulseCounter pulseCounter);
        }

        class PulseCounter
        {
            public long HighPulses { get; set; }
            public long LowPulses { get; set; }
        }

        class FlipFlopModule : Module
        {
            private bool IsOn = false;

            public FlipFlopModule(string name, string outputs)
            {
                Name = name;
                StringOutput = outputs;
                Type = "FlipFlopModule";
            }

            public override void ReceivePulse(string sender, bool isHighPulse, PulseCounter pulseCounter, Queue<Pulse> pulseQueue)
            {

                if (!isHighPulse)
                {
                    foreach (var output in Outputs)
                    {
                        pulseQueue.Enqueue(new Pulse { Module = output, IsHighPulse = IsOn, Caller = Name });

                        if (output.Type == "Conjunction")
                        {
                            (output as ConjunctionModule).inputModules[Name] = IsOn;
                        }
                        //Console.WriteLine($"{Name} ->  {(IsOn ? "high" : "low")} -> {output.Name}, {pulseCounter.HighPulses}");
                        output.PulseAction(Name, IsOn, pulseCounter);
                        if (IsOn) pulseCounter.HighPulses++;
                        else pulseCounter.LowPulses++;
                    }
                }

            }

            public override void PulseAction(string sender, bool isHighPulse, PulseCounter pulseCounter)
            {
                if (!isHighPulse)
                {
                    IsOn = !IsOn;

                }

            }
        }

        class ConjunctionModule : Module
        {
            public Dictionary<string, bool> inputModules = new Dictionary<string, bool>();
            private bool sendLowPulse = false;
            public ConjunctionModule(string name, string outputs)
            {
                Name = name;
                StringOutput = outputs;
                Type = "Conjunction";
            }

            public override void ReceivePulse(string sender, bool isHighPulse, PulseCounter pulseCounter, Queue<Pulse> pulseQueue)
            {
                foreach (var output in Outputs)
                {
                    pulseQueue.Enqueue(new Pulse { Module = output, IsHighPulse = !sendLowPulse, Caller = Name });
                    if (output.Type == "Conjunction")
                    {
                        (output as ConjunctionModule).inputModules[Name] = !sendLowPulse;
                    }
                    //Console.WriteLine($"{Name} ->  {(!sendLowPulse ? "high" : "low")} -> {output.Name}, {pulseCounter.HighPulses}");
                    output.PulseAction(Name, !sendLowPulse, pulseCounter);
                    if (!sendLowPulse) pulseCounter.HighPulses++;
                    else pulseCounter.LowPulses++;
                }
            }

            public override void PulseAction(string sender, bool isHighPulse, PulseCounter pulseCounter)
            {
                sendLowPulse = inputModules.Values.All(c => c);
            }
        }

        class BroadcasterModule : Module
        {
            public BroadcasterModule(string name, string outputs)
            {
                Name = name;
                StringOutput = outputs;
                Type = "Broadcaster";
            }
            public override void ReceivePulse(string sender, bool isHighPulse, PulseCounter pulseCounter, Queue<Pulse> pulseQueue)
            {


                foreach (var output in Outputs)
                {
                    //if (output.Type == "Conjunction")
                    //{
                    //    (output as ConjunctionModule).inputModules[Name] = isHighPulse;
                    //}
                    output.PulseAction(Name, isHighPulse, pulseCounter);
                    pulseQueue.Enqueue(new Pulse { Module = output, IsHighPulse = isHighPulse, Caller = Name });
                    if (isHighPulse) pulseCounter.HighPulses++;
                    else pulseCounter.LowPulses++;
                }
            }

            public override void PulseAction(string sender, bool isHighPulse, PulseCounter pulseCounter)
            {
            }
        }

        class UnTypeModule : Module
        {
            public UnTypeModule(string name, string outputs)
            {
                Name = name;
                StringOutput = outputs;
                Type = "UnType";
            }
            public override void ReceivePulse(string sender, bool isHighPulse, PulseCounter pulseCounter, Queue<Pulse> pulseQueue)
            {
                //Console.WriteLine($"{Name} -> {(isHighPulse ? "high" : "low")} -> ,  {pulseCounter.HighPulses}");
            }

            public override void PulseAction(string sender, bool isHighPulse, PulseCounter pulseCounter)
            {
            }
        }

        private long CalculateLCM(List<long> inputs)
        {
            if (inputs.Count >= 2)
            {
                return LCM(inputs[0], CalculateLCM(inputs.GetRange(1, inputs.Count - 1)));
            }

            return inputs[0];
        }

        private long LCM(long a, long b)
        {
            return a * b / GCD(a, b);
        }

        private long GCD(long a, long b)
        {
            if (b == 0)
                return a;
            return GCD(b, a % b);
        }

    }
}
