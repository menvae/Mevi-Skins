use std::ffi::{c_char, CStr, CString};
use std::panic::{self, AssertUnwindSafe};

use rgskin::export::fluxis::layout_to_dir;
use rgskin::utils::io::join_paths_unix;
use rgskin::FluXisSkin;
use rgskin::traits::ManiaSkin;
use sanitise_file_name::sanitise;

unsafe fn c_char_to_string(ptr: *const c_char) -> String {
    unsafe { CStr::from_ptr(ptr).to_string_lossy().into_owned() }
}

unsafe fn write_error(error_out: *mut *mut c_char, msg: impl ToString) {
    if !error_out.is_null() {
        let error_msg = CString::new(msg.to_string())
            .unwrap_or_else(|_| CString::new("Unknown error").unwrap());
        unsafe { *error_out = error_msg.into_raw() };
    }
}

unsafe fn ffi_catch<F>(f: F, error_out: *mut *mut c_char) -> bool
where
    F: FnOnce() -> Result<(), Box<dyn std::error::Error>>,
{
    let result = panic::catch_unwind(AssertUnwindSafe(|| f()));
    
    match result {
        Ok(Ok(_)) => true,
        Ok(Err(e)) => {
            unsafe { write_error(error_out, e) };
            false
        }
        Err(_) => {
            unsafe { write_error(error_out, "Function panicked") };
            false
        }
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn fromOsu(
    input_dir: *const c_char,
    skin_output_dir: *const c_char,
    export_layout: bool,
    layout_output_dir: *const c_char,
    error_out: *mut *mut c_char
) -> bool {
    unsafe {
        ffi_catch(|| {
            use rgskin::import::osu::skin_from_dir;
            use rgskin::export::fluxis::skin_to_dir;

            let input = c_char_to_string(input_dir);
            let output = c_char_to_string(skin_output_dir);
            let layout_output = c_char_to_string(layout_output_dir);

            let osu_skin = skin_from_dir(&input, false)?;
            let generic = osu_skin.to_generic_mania(())?;
            let skin = FluXisSkin::from_generic_mania(&generic)?;

            let skin_name = sanitise(&skin.0.skin_json.info.name);

            skin_to_dir(&skin.0, &output)?;

            if export_layout {
                layout_to_dir(&skin.1, &join_paths_unix(&layout_output, &format!("{skin_name}.json")))?;
            }
            
            Ok(())
        }, error_out)
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn free_error(ptr: *mut c_char) {
    if !ptr.is_null() {
        unsafe { drop(CString::from_raw(ptr)) };
    }
}
