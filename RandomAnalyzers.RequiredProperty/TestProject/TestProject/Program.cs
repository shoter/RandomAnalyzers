using LibraryProject;
using NugetProject;
using RandomAnalyzers.RequiredMember;
using System;

namespace TestProject
{
    public class InsideSameFile
    { 
        [RequiredMember]
        public int requiredProperty { get; set; }

        public int notRequired { get; set; }

        [RequiredMember]
        public int requiredField;

    }

    class Program
    {
        static void Main(string[] args)
        {

        }

        private static void diagnosticForNugetProject()
        {
            //should not trigger warning
            var test = new NugetFile()
            {
                requiredProperty = 123,
                requiredField = 456,
            };

            // should trigger warning - multiple prop msg
            new NugetFile();

            // should show multiple prop msg
            new NugetFile()
            {
                notRequired = 123,
            };

            // should show singular prop msg
            new NugetFile()
            {
                notRequired = 345,
                requiredProperty = 456,
            };
        }

        private static void diagnosticForExternalProject()
        {
            //should not trigger warning
            var test = new ExternalProject()
            {
                requiredProperty = 123,
                requiredField = 456,
            };

            // should trigger warning - multiple prop msg
            new ExternalProject();

            // should show multiple prop msg
            new ExternalProject()
            {
                notRequired = 123,
            };

            // should show singular prop msg
            new ExternalProject()
            {
                notRequired = 345,
                requiredProperty = 456,
            };
        }

        private static void diagnosticForExternalFile()
        {
            //should not trigger warning
            var test = new ExternalFile()
            {
                requiredProperty = 123,
                requiredField = 456,
            };

            // should trigger warning - multiple prop msg
            new ExternalFile();

            // should show multiple prop msg
            new ExternalFile()
            {
                notRequired = 123,
            };

            // should show singular prop msg
            new ExternalFile()
            {
                notRequired = 345,
                requiredProperty = 456,
            };
        }

        private static void diagnosticForTheSameFile()
        {
            // should not trigger warning
            var test = new InsideSameFile()
            {
                requiredField = 123,
                requiredProperty = 456,
            };

            // should trigger warning - multiple prop msg
            new InsideSameFile();

            // should show multiple prop msg
            new InsideSameFile()
            {
                notRequired = 123,
            };

            // should show singular prop msg
            new InsideSameFile()
            {
                notRequired = 345,
                requiredProperty = 456,
            };

            // should show singular prop msg
            new InsideSameFile()
            {
                notRequired = 345,
                requiredField = 456,
            };
        }
    }
}
