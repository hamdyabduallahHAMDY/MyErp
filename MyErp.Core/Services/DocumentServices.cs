using AutoMapper;
using Logger;
using MyErp.Core.DTO;
using MyErp.Core.Global;
using MyErp.Core.HTTP;
using MyErp.Core.Interfaces;
using MyErp.Core.Models;
using MyErp.Core.Validation;

namespace MyErp.Core.Services;

public class DocumentServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    Errors<Document> Errors = new Errors<Document>();

    public DocumentServices(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // =========================
    // Get All Documents
    // =========================
    public async Task<MainResponse<Document>> getDocumentList()
    {
        MainResponse<Document> response = new MainResponse<Document>();

        var documents = await _unitOfWork.Documents.GetAll();
        response.acceptedObjects = documents.ToList();

        return response;
    }

    // =========================
    // Get Document By ID
    // =========================
    public async Task<MainResponse<Document>> getDocument(int id)
    {
        MainResponse<Document> response = new MainResponse<Document>();

        var document = await _unitOfWork.Documents.GetById(id);

        if (document == null)
        {
            string error = Errors.ObjectNotFound();
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Document> { document };
        return response;
    }

    // Add Document
    public async Task<MainResponse<Document>> addDocument(
      DocumentDTO dto,
      string apiRootPath)
    {
        MainResponse<Document> response = new MainResponse<Document>();

        try
        {


            response.acceptedObjects = new List<Document>();

            string savedFilePath = null;

            if (dto.Attachment != null && dto.Attachment.Length > 0)
            {
                // Create Uploads folder inside API root
                string uploadsFolder = Path.Combine(apiRootPath, "Upload_Document");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Create unique filename

                string fullPath = Path.Combine(uploadsFolder ,  dto.Attachment.FileName    /*uniqueFileName*/);

                // Save file physically
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.Attachment.CopyToAsync(stream);
                }

                // Save relative path in DB
                savedFilePath = Path.Combine("Upload_Document" /*uniqueFileName*/);
            }

            Document document = new Document
            {
                Name = dto.Name,
                Attachment = dto.Attachment.FileName  
            };

            await _unitOfWork.Documents.Add(document);
            //await _unitOfWork.sav();

            response.acceptedObjects.Add(document);

            return response;
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());

            response.errors = new List<string> { ex.Message };

            if (ex.InnerException != null)
                response.errors.Add(ex.InnerException.Message);

            return response;
        }
    }

    // Update Document
    public async Task<MainResponse<Document>> updateDocument(int id, DocumentDTO documentUpdated)
    {
        var response = new MainResponse<Document>();

        try
        {
            var validList = await ValidateDTO.DocumentDTO(documentUpdated, true);

            var existingDocument =
                await _unitOfWork.Documents.GetFirst(a => a.Id == id);

            if (existingDocument is null)
            {
                response.errors.Add($"Cannot find Document with ID {id}.");

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(
                        _mapper.Map<List<Document>>(validList.rejectedObjects));

                return response;
            }

            if (validList.acceptedObjects is null ||
                validList.acceptedObjects.Count == 0)
            {
                response.errors.Add("No valid payload to update Document. Fix validation errors and try again.");

                if (validList.errors?.Any() == true)
                    response.errors.AddRange(validList.errors);

                if (validList.rejectedObjects?.Any() == true)
                    response.rejectedObjects.AddRange(
                        _mapper.Map<List<Document>>(validList.rejectedObjects));

                return response;
            }

            var dto = validList.acceptedObjects[0];

            _mapper.Map(dto, existingDocument);

            await _unitOfWork.Documents.Update(existingDocument);

            response.acceptedObjects.Add(existingDocument);

            if (validList.rejectedObjects?.Any() == true)
                response.rejectedObjects.AddRange(
                    _mapper.Map<List<Document>>(validList.rejectedObjects));

            if (validList.errors?.Any() == true)
                response.errors.AddRange(validList.errors);
        }
        catch (Exception ex)
        {
            Logs.Log(ex.ToString());
            response.errors.Add(ex.Message);
            if (ex.InnerException != null)
                response.errors.Add(ex.InnerException.Message);
        }

        return response;
    }

    // =========================
    // Delete Document
    // =========================
    public async Task<MainResponse<Document>> deleteDocument(int id)
    {
        MainResponse<Document> response = new MainResponse<Document>();

        var document =
            await _unitOfWork.Documents.DeletePhysical(p => p.Id == id);

        if (document == null)
        {
            string error = Errors.ObjectNotFoundWithId(id);
            response.errors = new List<string> { error };
            return response;
        }

        response.acceptedObjects = new List<Document> { document.First() };

        return response;
    }
}