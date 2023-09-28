
using xingyi.common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using xingyi.common.validator;
using Microsoft.AspNetCore.Mvc;
using xingyi.relationships;

namespace relationshipApi
{
    [ApiController]
    [Route("relationships")]

    public class RelationshipController : ControllerBase
    {
        readonly IRelationshipFinder finder;

        public RelationshipController(IRelationshipFinder finder)
        {
            this.finder = finder;
        }

        [HttpGet("subject/{source}/{nameSpace}/{name}")]
        public async Task<IActionResult> FindBySubject(string source, string nameSpace, string name)
        {
            return Ok(await finder.findBySubject(new Entity(source, nameSpace, name)));
        }
        [HttpGet("subject/{source}/{nameSpace}/{name}/relation/{relNs}/{relName}")]
        public async Task<IActionResult> FindBySubjectAnd(string source, string nameSpace, string name, string relNs, string relName)
        {
            return Ok(await finder.findBySubjectAnd(new Entity(source, nameSpace, name), new Relation(relNs, relName)));
        }
        [HttpGet("object/{source}/{nameSpace}/{name}")]
        public async Task<IActionResult> FindByObject(string source, string nameSpace, string name)
        {
            return Ok(await finder.findByObject(new Entity(source, nameSpace, name)));
        }
        [HttpGet("object/{source}/{nameSpace}/{name}/relation/{relNs}/{relName}")]
        public async Task<IActionResult> FindByObjectAnd(string source, string nameSpace, string name, string relNs, string relName)
        {
            return Ok(await finder.findByObjectAnd(new Entity(source, nameSpace, name), new Relation(relNs, relName)));
        }
        [HttpGet("object/{source}/{nameSpace}/{name}/relation/{relNs}/{relName}/object/{objSource}/{objNs}/{objName}")]
        public async Task<IActionResult> FindBySubjectAndObject(string source, string nameSpace, string name,
            string relNs, string relName, string objSource, string objNs, string objName)
        {
            return Ok(await finder.findBySubjectAndObject(
                new Entity(source, nameSpace, name),
                new Relation(relNs, relName),
                new Entity(objSource, objNs, objName)));
        }
    }
}
