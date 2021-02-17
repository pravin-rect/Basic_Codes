# get_doc_info
import os
from PyPDF2 import PdfFileReader, PdfFileWriter, PdfFileMerger

def get_info(path):
    with open(path, 'rb') as f:
        pdf = PdfFileReader(f)
        info = pdf.getDocumentInfo()
        number_of_pages = pdf.getNumPages()
    
    print(info)
    author = info.author
    creator = info.creator
    producer = info.producer
    subject = info.subject
    title = info.title
    print(number_of_pages)

#Function to split pdf file
def Split_PDF(path, Rnglist):
    #Check if the file exists
    if(not os.path.exists(path)):
        print('File does not exist')
        return False
    
    #Open PDF or throw message in case opening fails
    pdf = object
    try:
        #with open(path, 'rb') as f:
        pdf = PdfFileReader(open(path, 'rb'))
    except:
        print('Error opening file')
        return False
    
    #Extarct first/constant path of all new files
    FilNamWOExt = os.path.splitext(path)[0]
    AllPages = [item for item in range(pdf.getNumPages())]
    PageNotFound = False
    nCou = 0
    for Rng in Rnglist:
        #Using start and end page numbers populate the list of pages to include in a file
        St = int(Rng.split('-')[0])
        En = int(Rng.split('-')[1])
        En = En + 1 if En > St else En - 1
        Pages = [item for item in range(St,En,-1 if St > En else 1)]
        #print(Pages)

        #Create final file name and pdf writer object and add all required pages into it
        PageNotFoundInFile = False
        FilNam = '{}_Pages_{}-{}.pdf'.format(FilNamWOExt,Rng.split('-')[0],Rng.split('-')[1])
        PdfWrite = PdfFileWriter()
        for Page in Pages:
            if(Page - 1 in AllPages):
                PdfWrite.addPage(pdf.getPage(Page-1))
            else:
                PageNotFound = True
                PageNotFoundInFile = True
        if(PageNotFoundInFile):
            FilNam = '{}_MISSING PAGES.pdf'.format(os.path.splitext(FilNam)[0])
        with open(FilNam,'wb') as out:
            PdfWrite.write(out)
        print('Created {}'.format(FilNam))
        nCou += 1
    if(PageNotFound):
        print('Page numbers specified outside the existing range is omited')
    print('{} Files created successfully'.format(nCou))
    return True

#Function to parse page numbet
def Parse_PageNumbers(SplitPgNum, Rnglist):
    Ranges = str(SplitPgNum).split(',')
    if (len(Ranges) <= 0):
        print('Invalid page numbers')
        return False
    else:
        for Rng in Ranges:
            Pages = Rng.split('-')
            if (len(Pages) <= 0 or len(Pages) > 2):
                print('Invalid page numbers')
                return False
            for Page in Pages:
                if(not Page.strip().isdigit() or int(Page.strip()) <= 0): #isinstance(Page,int)
                    print('Invalid page numbers')
                    return False
            if(len(Pages) == 1):
                Rnglist.append(Pages[0].strip() + '-' + Pages[0].strip())
            else:
                Rnglist.append(Pages[0].strip() + '-' + Pages[1].strip())
    print(Rnglist)
    return True

def Merge_PDFs(Inp_Fils,Out_path):
    #Split input strings to multiple files
    Fils = Inp_Fils.split(',')
    if(len(Fils) < 0):
        print('No proper input for input files')
        return False
    else:
        for Fil in Fils:
            if(not os.path.exists(Fil)):
                print('One or more files not found')
                return False

    try:
        pdf_merger = PdfFileMerger()
        for path in Fils:
            pdf_merger.append(path)
        with open(os.path.join(Out_path,'{}_MERGED.pdf'.format(os.path.splitext(Fils[0])[0])), 'wb') as f:
            pdf_merger.write(f)
    except:
        print('Improper inputs. Merging failed')
        return False
    print('{} Files Merged'.format(len(Fils)))

def ProvideChoice():
    #Provide Choice
    print('1. Split\n2. Merge')
    Inp = input('Enter the choice: ')
    if(not Inp.isdigit() or int(Inp) <= 0 or int(Inp) > 2):
        return False
    
    if(int(Inp) == 1):
        #PDF file name and pages to split
        PDF_path=input('Enter full file path: ')
        Pages=input('Enter to page numbers to split: ')

        #Parse page numbers and split file
        RngList = []
        if (Parse_PageNumbers(Pages,RngList)):
            Split_PDF(PDF_path, RngList)
    elif(int(Inp) == 2):
        #PDF file names and location to place file
        Filstr=input('Enter PDF file paths to merge: ')
        PDF_path=input('Enter full path: ')
        Merge_PDFs(Filstr, PDF_path)

#Main if
if __name__ == '__main__':
    #get_info(PDF_path)
    ProvideChoice()