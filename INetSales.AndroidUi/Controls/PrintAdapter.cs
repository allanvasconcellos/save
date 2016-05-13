using System;
using Android.Print;
using Java.IO;
using Android.Print.Pdf;

namespace INetSales.AndroidUi.Controls
{
	public class PrintAdapter : PrintDocumentAdapter
	{
		File _file;
		PrintedPdfDocument mPdfDocument;
		BaseActivity activity;
		
		public PrintAdapter (BaseActivity activity, string documentoFile)
		{
			this.activity = activity;
			_file = new File (documentoFile);
		}

		#region implemented abstract members of PrintDocumentAdapter

		public override void OnLayout (PrintAttributes oldAttributes, PrintAttributes newAttributes, Android.OS.CancellationSignal cancellationSignal, LayoutResultCallback callback, Android.OS.Bundle extras)
		{
			// Create a new PdfDocument with the requested page attributes
			mPdfDocument = new PrintedPdfDocument(activity.BaseContext, newAttributes);

			// Respond to cancellation request
			if (cancellationSignal.IsCanceled ) {
				callback.OnLayoutCancelled();
				return;
			}

			// Compute the expected number of printed pages
			int pages = ComputePageCount(newAttributes);

			if (pages > 0) {
				// Return print information to print framework
				PrintDocumentInfo info = new PrintDocumentInfo
					.Builder(_file.AbsolutePath)
					.SetContentType(PrintContentType.Document)
					.SetPageCount(pages)
					.Build();
				// Content layout reflow is complete
				callback.OnLayoutFinished(info, true);
			} else {
				// Otherwise report an error to the print framework
				callback.OnLayoutFailed("Page count calculation failed.");
			}
		}

		public override void OnWrite (PageRange[] pages, Android.OS.ParcelFileDescriptor destination, Android.OS.CancellationSignal cancellationSignal, WriteResultCallback callback)
		{
			// Iterate over each page of the document,
			// check if it's in the output range.
			for (int i = 0; i < totalPages; i++) {
				// Check to see if this page is in the output range.
				if (ContainsPage(pageRanges, i)) {	
					// If so, add it to writtenPagesArray. writtenPagesArray.size()
					// is used to compute the next output page index.
					writtenPagesArray.append(writtenPagesArray.size(), i);
					PdfDocument.Page page = mPdfDocument.startPage(i);

					// check for cancellation
					if (cancellationSignal.isCancelled()) {
						callback.onWriteCancelled();
						mPdfDocument.close();
						mPdfDocument = null;
						return;
					}

					// Draw page content for printing
					drawPage(page);

					// Rendering is complete, so page can be finalized.
					mPdfDocument.finishPage(page);
				}
			}

			// Write PDF document to file
			try {
				mPdfDocument.writeTo(new FileOutputStream(
					destination.getFileDescriptor()));
			} catch (IOException e) {
				callback.onWriteFailed(e.toString());
				return;
			} finally {
				mPdfDocument.close();
				mPdfDocument = null;
			}
			PageRange[] writtenPages = computeWrittenPages();
			// Signal the print framework the document is complete
			callback.onWriteFinished(writtenPages);
		}

		#endregion

		private int ComputePageCount(PrintAttributes printAttributes) {
			int itemsPerPage = 4; // default item count for portrait mode

			Android.Print.PrintAttributes.MediaSize pageSize = printAttributes.GetMediaSize();
			if (!pageSize.IsPortrait) {
				// Six items per page in landscape orientation
				itemsPerPage = 6;
			}

			// Determine number of print items
			int printItemCount = GetPrintItemCount();

			return (int) Math.Ceiling(printItemCount / itemsPerPage);
		}
	}
}

