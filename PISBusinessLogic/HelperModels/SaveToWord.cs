using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace PISBusinessLogic.HelperModels
{
    public class SaveToWord
    {
        public static void CreateDoc(WordInfo info)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(info.FileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body docBody = mainPart.Document.AppendChild(new Body());
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { info.Title },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = true,
                        Size = "24",
                        JustificationValues = JustificationValues.Center
                    }
                }));
                if (info.book == null)
                {
                    int year = Convert.ToInt32(info.libraryCard.Year) + 1;
                    Table table = new Table();
                    TableProperties tblProp = new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 8 }
                        )
                    );
                    table.AppendChild<TableProperties>(tblProp);
                    TableRow row1 = new TableRow();
                    TableCell header1 = new TableCell(new Paragraph(new Run(new Text("ФИО читателя: " + info.libraryCard.ReaderFIO))));
                    TableCell header2 = new TableCell(new Paragraph(new Run(new Text("Записи"))));
                    row1.Append(header1);
                    row1.Append(header2);
                    table.Append(row1);
                    TableRow row2 = new TableRow();
                    TableCell header3 = new TableCell(new Paragraph(new Run(new Text("Дата рождения: " + info.libraryCard.DateOfBirth.ToShortDateString()))));
                    TableCell header4 = new TableCell(new Paragraph(new Run(new Text(""))));
                    row2.Append(header3);
                    row2.Append(header4);
                    table.Append(row2);
                    TableRow row3 = new TableRow();
                    TableCell header5 = new TableCell(new Paragraph(new Run(new Text("Место работы: " + info.libraryCard.PlaceOfWork))));
                    TableCell header6 = new TableCell(new Paragraph(new Run(new Text(""))));
                    row3.Append(header5);
                    row3.Append(header6);
                    table.Append(row3);
                    TableRow row4 = new TableRow();
                    TableCell header7 = new TableCell(new Paragraph(new Run(new Text("Действителен до:" + year))));
                    TableCell header8 = new TableCell(new Paragraph(new Run(new Text(""))));
                    row4.Append(header7);
                    row4.Append(header8);
                    table.Append(row4);
                    docBody.Append(table);
                }
                if (info.libraryCard == null)
                {
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Название: "+info.book.Name },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> {"Автор: "+ info.book.Author },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> {"Издательство: "+ info.book.PublishingHouse },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Год: " +info.book.Year },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Дата формирования справки: " + DateTime.Now.ToShortDateString()},
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));               
                }
                wordDocument.MainDocumentPart.Document.Save();
            }
        }
        public static void CreateDoc(WordInfoList info)
        {
            using (WordprocessingDocument wordDocument1 = WordprocessingDocument.Create(info.FileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument1.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body docBody = mainPart.Document.AppendChild(new Body());
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { info.Title },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = true,
                        Size = "24",
                        JustificationValues = JustificationValues.Center
                    }
                }));
                int i = 0;
                foreach(var us in info.UserFIO)
                {
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Библиотекарь: " + us.FIO},
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    i++;
                }
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { "Дата формирования: " + DateTime.Now.ToShortDateString() },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = false,
                        Size = "24",
                        JustificationValues = JustificationValues.Left
                    }
                }));

                wordDocument1.MainDocumentPart.Document.Save();
            }
        }
        public static void CreateDoc(WordInfoContract info)
        {
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(info.FileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body docBody = mainPart.Document.AppendChild(new Body());
                docBody.AppendChild(CreateParagraph(new WordParagraph
                {
                    Texts = new List<string> { info.Title },
                    TextProperties = new WordParagraphProperties
                    {
                        Bold = true,
                        Size = "24",
                        JustificationValues = JustificationValues.Center
                    }
                }));

                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "ФИО: " + info.user.FIO },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Должность: " + info.user.Role },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Оклад: " + info.user.Salary },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Дата принятия на работу: 01.12.2020" },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                    docBody.AppendChild(CreateParagraph(new WordParagraph
                    {
                        Texts = new List<string> { "Главный бухгалтер: Ефимова" },
                        TextProperties = new WordParagraphProperties
                        {
                            Bold = false,
                            Size = "24",
                            JustificationValues = JustificationValues.Left
                        }
                    }));
                wordDocument.MainDocumentPart.Document.Save();
            }
        }
        private static SectionProperties CreateSectionProperties()
        {
            SectionProperties properties = new SectionProperties();
            PageSize pageSize = new PageSize
            {
                Orient = PageOrientationValues.Portrait
            };
            properties.AppendChild(pageSize);
            return properties;
        }
        private static Paragraph CreateParagraph(WordParagraph paragraph)
        {
            if (paragraph != null)
            {
                Paragraph docParagraph = new Paragraph();
                docParagraph.AppendChild(CreateParagraphProperties(paragraph.TextProperties));
                foreach (var run in paragraph.Texts)
                {
                    Run docRun = new Run();
                    RunProperties properties = new RunProperties();
                    properties.AppendChild(new FontSize
                    {
                        Val = paragraph.TextProperties.Size
                    });
                    if (paragraph.TextProperties.Bold)
                    {
                        properties.AppendChild(new Bold());
                    }
                    docRun.AppendChild(properties);
                    docRun.AppendChild(new Text
                    {
                        Text = run,
                        Space = SpaceProcessingModeValues.Preserve
                    });
                    docParagraph.AppendChild(docRun);
                }
                return docParagraph;
            }
            return null;
        }
        private static ParagraphProperties
        CreateParagraphProperties(WordParagraphProperties paragraphProperties)
        {
            if (paragraphProperties != null)
            {
                ParagraphProperties properties = new ParagraphProperties();
                properties.AppendChild(new Justification()
                {
                    Val = paragraphProperties.JustificationValues
                });
                properties.AppendChild(new SpacingBetweenLines
                {
                    LineRule = LineSpacingRuleValues.Auto
                });
                properties.AppendChild(new Indentation());
                ParagraphMarkRunProperties paragraphMarkRunProperties = new ParagraphMarkRunProperties();
                if (!string.IsNullOrEmpty(paragraphProperties.Size))
                {
                    paragraphMarkRunProperties.AppendChild(new FontSize
                    {
                        Val = paragraphProperties.Size
                    });
                }
                if (paragraphProperties.Bold)
                {
                    paragraphMarkRunProperties.AppendChild(new Bold());
                }
                properties.AppendChild(paragraphMarkRunProperties);
                return properties;
            }
            return null;
        }
    }
}