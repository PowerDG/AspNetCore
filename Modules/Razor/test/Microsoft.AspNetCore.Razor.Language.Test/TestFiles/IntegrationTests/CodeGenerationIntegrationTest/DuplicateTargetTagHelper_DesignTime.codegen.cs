// <auto-generated/>
#pragma warning disable 1591
namespace Microsoft.AspNetCore.Razor.Language.IntegrationTests.TestFiles
{
    #line hidden
    public class TestFiles_IntegrationTests_CodeGenerationIntegrationTest_DuplicateTargetTagHelper_DesignTime
    {
        private global::TestNamespace.InputTagHelper __TestNamespace_InputTagHelper;
        private global::TestNamespace.CatchAllTagHelper __TestNamespace_CatchAllTagHelper;
        #pragma warning disable 219
        private void __RazorDirectiveTokenHelpers__() {
        ((System.Action)(() => {
global::System.Object __typeHelper = "*, TestAssembly";
        }
        ))();
        }
        #pragma warning restore 219
        #pragma warning disable 0414
        private static System.Object __o = null;
        #pragma warning restore 0414
        #pragma warning disable 1998
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            __TestNamespace_InputTagHelper = CreateTagHelper<global::TestNamespace.InputTagHelper>();
            __TestNamespace_CatchAllTagHelper = CreateTagHelper<global::TestNamespace.CatchAllTagHelper>();
            __TestNamespace_InputTagHelper.Type = "checkbox";
            __TestNamespace_CatchAllTagHelper.Type = __TestNamespace_InputTagHelper.Type;
#line 3 "TestFiles/IntegrationTests/CodeGenerationIntegrationTest/DuplicateTargetTagHelper.cshtml"
__TestNamespace_InputTagHelper.Checked = true;

#line default
#line hidden
            __TestNamespace_CatchAllTagHelper.Checked = __TestNamespace_InputTagHelper.Checked;
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
