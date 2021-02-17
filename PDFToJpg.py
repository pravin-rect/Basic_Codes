from PyPDF2 import PdfFileReader, PdfFileWriter
import io
import os
from PIL import Image
from pdf2image import convert_from_path

#Using PyPdf2
def SaveAsJpg():
    #PDF file name and pages to save as jpeg
    path=input('Enter full file path:')

    #Open PDF or throw message in case opening fails
    pdf = object
    try:
        #with open(path, 'rb') as f:
        pdf = PdfFileReader(open(path, 'rb'),strict=False)
    except:
        print('Error opening file')
        return False

    #get file name without extension get all pagenumbers
    FilNamWOExt = os.path.split(path)[0]
    AllPages = [item for item in range(pdf.getNumPages())]

    # Set pdf writer to 
    bytstr = object
    Writr = object
    img = object
    for PgNum in AllPages:
        Writr = PdfFileWriter()
        Writr.addPage(pdf.getPage(PgNum))
        #set as byte stream
        bytstr = io.BytesIO()
        Writr.write(bytstr)
        bytstr.seek(0)
        img = Image.open(bytstr)
        img.SaveAsJpg('{}_{}.jpg'.format(FilNamWOExt,PgNum))
        bytstr.flush()

#using py2Image
def SaveAsJpg1():
    #PDF file name and pages to save as jpeg
    path=input('Enter full file path:')
    FilNamWOExt = path.split('.')[0]
    nCou = 0

    try:
        imgs = convert_from_path(path)
        for img in imgs:
            nCou += 1
            img.save('{}_{}.jpg'.format(FilNamWOExt, nCou),'JPEG')
    except:
        print('Error opening file')
        return False

if __name__ == "__main__":
    SaveAsJpg1()