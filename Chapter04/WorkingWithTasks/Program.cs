﻿using System.Diagnostics;

Stopwatch timer = Stopwatch.StartNew();

SectionTitle("Running methods asynchrously on one thread");

MethodA();
MethodB();
MethodC();

WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed");

SectionTitle("Running methods asynchronously on multiple threads");
timer.Restart();

Task taskA = new(MethodA);
taskA.Start();
Task taskB = Task.Factory.StartNew(MethodB);
Task taskC = Task.Run(MethodC);

Task[] myTasks = {taskA, taskB, taskC};

Task.WaitAll(myTasks);

WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed");

SectionTitle("Passing the result of one task as an input into another");
timer.Restart();

Task<string> taskServiceThenSproc = Task.Factory
.StartNew(CallWebService)
.ContinueWith(previousTask => CallStoredProcedure(previousTask.Result));

WriteLine($"Result: {taskServiceThenSproc.Result}");
WriteLine($"{timer.ElapsedMilliseconds:#,##0}ms elapsed.");

SectionTitle("Nested and Child Tasks");
Task outerTask = Task.Factory.StartNew(OuterMethod);
outerTask.Wait();
WriteLine("Console app is stopping");
