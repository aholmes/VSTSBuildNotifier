using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VSTS.Contract
{
	public class Build
	{
		//
		// Summary:
		//     Build number/name of the build
		[DataMember(EmitDefaultValue = false)]
		public string BuildNumber { get; set; }
		//
		// Summary:
		//     Build number revision
		[DataMember(EmitDefaultValue = false)]
		public int? BuildNumberRevision { get; set; }
		//
		// Summary:
		//     Indicates whether the build has been deleted.
		[DataMember(EmitDefaultValue = false)]
		public bool Deleted { get; set; }
		//
		// Summary:
		//     Time that the build was completed
		[DataMember(EmitDefaultValue = false)]
		public DateTime? FinishTime { get; set; }
		//
		// Summary:
		//     Id of the build
		[DataMember(EmitDefaultValue = false)]
		[System.ComponentModel.DataAnnotations.KeyAttribute]
		public int Id { get; set; }
		[DataMember(EmitDefaultValue = false)]
		public bool? KeepForever { get; set; }
		//
		// Summary:
		//     Parameters for the build
		[DataMember(EmitDefaultValue = false)]
		public string Parameters { get; set; }
		//
		// Summary:
		//     Quality of the xaml build (good, bad, etc.)
		[DataMember(IsRequired = false, EmitDefaultValue = false)]
		public string Quality { get; set; }
		//
		// Summary:
		//     The current position of the build in the queue
		[DataMember(EmitDefaultValue = false)]
		public int? QueuePosition { get; set; }
		//
		// Summary:
		//     Time that the build was queued
		[DataMember(EmitDefaultValue = false)]
		public DateTime? QueueTime { get; set; }
		//
		// Summary:
		//     The build result
		[DataMember(EmitDefaultValue = false)]
		public Contract.BuildResult? Result { get; set; }
		//
		// Summary:
		//     Source branch
		[DataMember(EmitDefaultValue = false)]
		public string SourceBranch { get; set; }
		//
		// Summary:
		//     Source version
		[DataMember(EmitDefaultValue = false)]
		public string SourceVersion { get; set; }
		//
		// Summary:
		//     Time that the build was started
		[DataMember(EmitDefaultValue = false)]
		public DateTime? StartTime { get; set; }
		//
		// Summary:
		//     Status of the build
		[DataMember(EmitDefaultValue = false)]
		public Contract.BuildStatus? Status { get; set; }
		//
		// Summary:
		//     Gets a collection of tags associated with the build.
		public List<string> Tags { get; }
		//
		// Summary:
		//     Uri of the build
		[DataMember(EmitDefaultValue = false)]
		public Uri Uri { get; set; }
		//
		// Summary:
		//     REST url of the build
		[DataMember(EmitDefaultValue = false)]
		public string Url { get; set; }
		//
		// Summary:
		//     The name on whose behalf the build was queued
		[DataMember(EmitDefaultValue = false)]
		public string RequestedFor { get; set; }
	}
}
