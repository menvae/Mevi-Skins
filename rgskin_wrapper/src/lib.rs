use std::ffi::{c_char, CStr};

use rgskin::{traits::ManiaSkin, FluXisSkin};

unsafe fn c_char_to_string(ptr: *const c_char) -> String {
    unsafe { CStr::from_ptr(ptr)
        .to_string_lossy()
        .into_owned() }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn fromOsu(input_dir: *const c_char, output_dir: *const c_char) {
    use rgskin::import::osu::skin_from_dir;
    use rgskin::export::fluxis::skin_to_dir;

    let input = unsafe { &c_char_to_string(input_dir) };
    let output = unsafe { &c_char_to_string(output_dir) };

    let osu_skin = skin_from_dir(input).unwrap();
    let generic = osu_skin.to_generic_mania(()).unwrap();
    let skin = FluXisSkin::from_generic_mania(&generic).unwrap();
    skin_to_dir(&skin.0, output).unwrap();
}