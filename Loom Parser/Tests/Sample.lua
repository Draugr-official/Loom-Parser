

du.check_min_api_version("5.0.2", "darkroom_demo") 

local function destroy()
    
end


local MODULE_NAME = "darkroom_demo"

local gettext = dt.gettext.gettext

local function _(msgid)
  return gettext(msgid)
end

local sleep = dt.control.sleep


local current_view = dt.gui.current_view()


local images = dt.gui.action_images
dt.print_log(#images .. " images selected")
if images or #images == 0 then
  dt.print_log("no images selected, creating selection")
end

dt.gui.current_view(dt.gui.views.darkroom)

local max_images = 10

dt.print(_("showing images, with a pause between each"))
sleep(1500)

for i, img in ipairs(dt.collection) do 
  dt.print(string.format(_("displaying image "), i))
  dt.gui.views.darkroom.display_image(img)
  sleep(1500)
  if i == max_images then
    break
  end
end

dt.print(_("restoring view"))
sleep(1500)
dt.gui.current_view(current_view)

local script_data = {}

script_data.metadata = {
  name = _("darkroom demo"),
  purpose = _("example demonstrating how to control image display in darkroom mode"),
  author = "Bill Ferguson <wpferguson@gmail.com>",
  help = "https://docs.darktable.org/lua/stable/lua.scripts.manual/scripts/examples/darkroom_demo"
}

script_data.destroy = destroy

return script_data